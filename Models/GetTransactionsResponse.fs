namespace TransactionSorter.Models

open System.Text.Json.Serialization

type GetTransactionsResponse () =
    [<JsonPropertyName("data")>]
    member val Data : MultipleTransactionData = MultipleTransactionData() with get, set
