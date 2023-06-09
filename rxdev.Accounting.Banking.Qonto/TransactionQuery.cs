using System.Text.Json.Serialization;

namespace rxdev.Accounting.Banking.Qonto;

internal class TransactionQuery
{
    [JsonPropertyName("meta")]
    public Meta? Meta { get; set; }

    [JsonPropertyName("transactions")]
    public Transaction[]? Transactions { get; set; }
}