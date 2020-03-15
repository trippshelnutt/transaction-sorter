namespace TransactionSorter.Controllers

open System
open System.Collections.Generic
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
            use client = new HttpClient()
            client.DefaultRequestHeaders.Accept.Clear()
            client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
            client.DefaultRequestHeaders.Add("Authorization", "BEARER " + configuration.["YNAB:apikey"])
        
            let! message = client.GetStringAsync(configuration.["YNAB:URL"] + "/budgets") |> Async.AwaitTask 
            return this.Content(message)
        }
