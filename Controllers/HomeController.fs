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
            let! message = client.GetStringAsync(requestUrl) |> Async.AwaitTask 
            return message
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
