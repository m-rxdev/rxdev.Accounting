using AutoMapper;
using System.Text.Json;

namespace rxdev.Accounting.Banking.Qonto;

public class QontoClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };
    private static readonly Uri BaseAddress = new("https://thirdparty.qonto.com/");

    private readonly IMapper _mapper;

    public QontoClient(IMapper mapper)
    {
        _mapper = mapper;
    }

    public List<Model.BankTransaction> GetTransactions(string iban, string authorization, DateTime? from = null, DateTime? to = null)
    {
        HttpClient client = new()
        {
            BaseAddress = BaseAddress,
        };
        client.DefaultRequestHeaders.Add("Accept", $"application/json, text/plain");
        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorization);

        List<Transaction> transactions = new();
        int currentPage = 1;

        TransactionQuery? query;
        string uri = $"/v2/transactions?sort_by=settled_at:asc&iban={iban}";

        if (from.HasValue)
            uri += $"&settled_at_from={from.Value.ToUniversalTime():o}";
        if(to.HasValue)
            uri += $"&settled_at_to={to.Value.ToUniversalTime():o}";

        do
        {
            Task<string> result = client.GetStringAsync(uri + $"&current_page={currentPage}");

            result.Wait();

            query = JsonSerializer.Deserialize<TransactionQuery>(result.Result, SerializerOptions);

            if (query == null)
                break;

            if (query.Transactions is not null)
                transactions.AddRange(query.Transactions);

            currentPage++;
        } while (query?.Meta?.NextPage != null);

        return _mapper.Map<IEnumerable<Model.BankTransaction>>(transactions).ToList();
    }
}