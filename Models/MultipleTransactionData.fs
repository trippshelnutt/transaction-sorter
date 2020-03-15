namespace TransactionSorter.Models

open System.Text.Json.Serialization
open System.Collections.Generic

type MultipleTransactionData () =
    [<JsonPropertyName("transactions")>]
    member val Transactions : List<TransactionModel> = List<TransactionModel>() with get, set
