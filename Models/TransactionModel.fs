namespace TransactionSorter.Models

open System.Text.Json.Serialization

type TransactionModel () =
    [<JsonPropertyName("amount")>]
    member val MilliunitAmount : int = 0 with get, set
    [<JsonPropertyName("payee_name")>]
    member val Payee : string = null with get, set
    [<JsonPropertyName("parent_transaction_id")>]
    member val ParentTransactionId : string = null with get, set
    member this.Amount : decimal =
        (decimal)this.MilliunitAmount / 1000.00m
