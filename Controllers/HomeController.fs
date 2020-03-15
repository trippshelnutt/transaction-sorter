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

type HomeController (logger : ILogger<HomeController>, configuration : IConfiguration) =
    inherit Controller()

    member this.Index () =
        this.View()

    member this.Privacy () =
        this.View()

    member this.Error () =
        this.View();

    member this.ApiTest () =
        async {
            let filterDate = DateTime.ParseExact("2020-2-1", "yyyy-M-d", CultureInfo.InvariantCulture)
            let! message = this.GetTransactions filterDate
            return this.Content(message)
        }

    member private this.GetTransactions (sinceDate : DateTime) =
        async {
            use client = this.BuildHttpClient()
        
            let groceriesId = configuration.["YNAB:Groceries"]
            let requestUrl = this.GetTransactionsByCategoryAndDateRequestUrl groceriesId sinceDate
            let! responseStream = client.GetStreamAsync(requestUrl) |> Async.AwaitTask 
            let! response = JsonSerializer.DeserializeAsync<GetTransactionsResponse>(responseStream).AsTask <| () |> Async.AwaitTask
            let content =
                response.Data.Transactions
                |> Seq.sortByDescending (fun t -> t.MilliunitAmount)
                |> Seq.map this.CheckAndFillParentInformation
                |> Async.Parallel
                |> Async.RunSynchronously
                |> Seq.map this.GetTransactionLineItem
                |> Seq.fold (+) ""
            return content
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
        sprintf "Payee: %30s Amount: %8.2f\n" transaction.Payee transaction.Amount

