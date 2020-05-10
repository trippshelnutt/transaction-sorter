namespace TransactionSorter

open System
open Microsoft.AspNetCore.Hosting
open Microsoft.Azure.KeyVault
open Microsoft.Azure.Services.AppAuthentication
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Configuration.AzureKeyVault
open Microsoft.Extensions.Hosting

module Program =
    let exitCode = 0
    
    let KeyVaultTokenCallback (callback:AzureServiceTokenProvider.TokenCallback) (authority:string) (resource:string) (scope:string) =
        callback.Invoke(authority, resource, scope)

    let CreateHostBuilder args =
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(fun context config ->
                let builtConfig = config.Build()
                let keyVaultEndpoint = builtConfig.["AzureKeyVault:URL"]
                if (not (String.IsNullOrEmpty keyVaultEndpoint)) then
                    let azureServiceTokenProvider = AzureServiceTokenProvider()
                    let keyVaultTokenCallback = azureServiceTokenProvider.KeyVaultTokenCallback |> KeyVaultTokenCallback
                    let authenticationCallback = new KeyVaultClient.AuthenticationCallback(keyVaultTokenCallback);
                    let keyVaultClient = new KeyVaultClient(authenticationCallback)
                    config.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, DefaultKeyVaultSecretManager()) |> ignore
            )
            .ConfigureWebHostDefaults(fun webBuilder ->
                webBuilder.UseStartup<Startup>() |> ignore
            )

    [<EntryPoint>]
    let main args =
        CreateHostBuilder(args).Build().Run()

        exitCode
