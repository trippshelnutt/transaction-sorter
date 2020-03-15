namespace TransactionSorter.Controllers

open System
open System.Collections.Generic
open System.Globalization
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration
open System.Net.Http
open System.Net.Http.Headers
open System.Text.Json
open TransactionSorter.Models

[<ApiController>]
type TransactionsController (logger : ILogger<HomeController>, configuration : IConfiguration) =
    inherit ControllerBase()

    [<HttpGet("/api/[controller]/{month}/{category}")>]
    member this.Index (month : int, category : string) =
        async {
            let now = DateTime.Now
            let startDate = DateTime(now.Year, month, 1)
            let endDate = startDate.AddMonths(1).AddDays(-1.0)
            let! transactions = this.GetTransactions startDate endDate category
            let multipleTransactionData = MultipleTransactionData()
            multipleTransactionData.Transactions <- transactions
            let getTransactionsResponse = GetTransactionsResponse()
            getTransactionsResponse.Data <- multipleTransactionData
            let options = JsonSerializerOptions()
            options.WriteIndented <- true
            return this.Content(JsonSerializer.Serialize(getTransactionsResponse, options))
        }

    member private this.GetTransactions (startDate : DateTime) (endDate : DateTime) (category : string) =
        async {
            use client = this.BuildHttpClient()
        
            let categorySetting = sprintf "YNAB:%s" category
            let categoryId = configuration.[categorySetting]
            let requestUrl = this.GetTransactionsByCategoryAndDateRequestUrl categoryId startDate
            let! responseStream = client.GetStreamAsync(requestUrl) |> Async.AwaitTask 
            let! response = JsonSerializer.DeserializeAsync<GetTransactionsResponse>(responseStream).AsTask <| () |> Async.AwaitTask
            let transactions =
                response.Data.Transactions
                |> Seq.filter (fun t -> t.Date <= endDate)
                |> Seq.sortByDescending (fun t -> t.MilliunitAmount)
                |> Seq.map this.CheckAndFillParentInformation
                |> Async.Parallel
                |> Async.RunSynchronously
            return transactions.ToList()
        }

    member private this.GetTransaction (transactionId : string) =
        async {
            use client = this.BuildHttpClient()
        
            let requestUrl = this.GetTransactionByIdRequestUrl transactionId
            let! responseStream = client.GetStreamAsync(requestUrl) |> Async.AwaitTask 
            let! response = JsonSerializer.DeserializeAsync<GetTransactionResponse>(responseStream).AsTask <| () |> Async.AwaitTask
            return response.Data.Transaction
        }
        
    member private this.BuildHttpClient () : HttpClient =
        let client = new HttpClient()
        client.DefaultRequestHeaders.Accept.Clear()
        client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
        client.DefaultRequestHeaders.Add("Authorization", "BEARER " + configuration.["YNAB:apikey"])
        client

    member private this.GetTransactionsByCategoryAndDateRequestUrl (categoryId : string) (sinceDate : DateTime) : Uri =
        let urlBase = configuration.["YNAB:URL"]
        let budgetId = configuration.["YNAB:Budget"]
        let sinceDateString = sinceDate.ToString("yyyy-M-d")
        Uri(sprintf "%s/budgets/%s/categories/%s/transactions?since_date=%s" urlBase budgetId categoryId sinceDateString)

    member private this.GetTransactionByIdRequestUrl (transactionId : string) : Uri =
        let urlBase = configuration.["YNAB:URL"]
        let budgetId = configuration.["YNAB:Budget"]
        Uri(sprintf "%s/budgets/%s/transactions/%s" urlBase budgetId transactionId)

    member private this.CheckAndFillParentInformation (transaction : TransactionModel) =
        async {
            match transaction.Payee with
                | p when p |> isNull -> return! this.FillParentInformation transaction
                | _ -> return transaction
        }

    member private this.FillParentInformation (transaction : TransactionModel) =
        async {
            let! parentTransaction = this.GetTransaction(transaction.ParentTransactionId)
            transaction.Payee <- parentTransaction.Payee
            return transaction
        }

    member private this.GetTransactionLineItem (transaction : TransactionModel) =
        let transactionDate = transaction.Date.ToString("yyyy-M-d")
        sprintf "Payee: %30s Amount: %8s Date: %s\n" transaction.Payee transaction.DisplayAmount transactionDate
