using System.Text.Json.Serialization;

namespace rxdev.Accounting.Banking.Qonto;

internal class Transaction
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("local_amount")]
    public decimal LocalAmount { get; set; }

    [JsonPropertyName("local_currency")]
    public string? LocalCurrency { get; set; }

    [JsonPropertyName("operation_type")]
    public string? OperationType { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("settled_at")]
    public DateTime SettledAt { get; set; }

    [JsonPropertyName("settled_balance")]
    public decimal SettledBalance { get; set; }

    [JsonPropertyName("transaction_id")]
    public string TransactionId { get; set; } = string.Empty;

    [JsonPropertyName("side")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransactionSide Side { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }
}