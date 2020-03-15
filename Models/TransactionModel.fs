namespace TransactionSorter.Models

open System.Text.Json.Serialization
open System

type TransactionModel () =
    [<JsonPropertyName("amount")>]
    member val MilliunitAmount : int = 0 with get, set
    [<JsonPropertyName("payee_name")>]
    member val Payee : string = null with get, set
    [<JsonPropertyName("parent_transaction_id")>]
    member val ParentTransactionId : string = null with get, set
    [<JsonPropertyName("date")>]
    member val Date : DateTime = DateTime() with get, set
    member this.DecimalAmount : decimal =
         ((decimal)this.MilliunitAmount / 1000.00m)
    member this.DisplayAmount : string =
        sprintf "$%.2f" this.DecimalAmount
