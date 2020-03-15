namespace TransactionSorter.Models

open System.Text.Json.Serialization

type GetTransactionResponse () =
    [<JsonPropertyName("data")>]
    member val Data : SingleTransactionData = SingleTransactionData() with get, set
