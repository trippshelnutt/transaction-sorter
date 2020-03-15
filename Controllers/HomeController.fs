namespace TransactionSorter.Controllers

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Configuration

type HomeController (logger : ILogger<HomeController>, configuration : IConfiguration) =
    inherit Controller()

    member this.Index () =
        this.View()
