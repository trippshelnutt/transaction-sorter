namespace TransactionSorter.Models

open System.Text.Json.Serialization

type SingleTransactionData () =
    [<JsonPropertyName("transaction")>]
    member val Transaction : TransactionModel = TransactionModel() with get, set
