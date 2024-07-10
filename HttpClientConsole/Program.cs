using HttpClientConsole.Constants;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(SendWithdrawRequestAsync());
        }

        await Task.WhenAll(tasks);
    }

    static async Task SendWithdrawRequestAsync()
    {
        using HttpClient httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });

        Transaction withdraw = new Transaction { AccountId = new Guid("542646b9-5531-479c-a6c1-0a714b049259"), Amount = 150000 };

        var withdrawJson = JsonSerializer.Serialize(withdraw);

        var request = new HttpRequestMessage(HttpMethod.Post, ConfigurationSettings.Uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(withdrawJson, Encoding.UTF8);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Headers.Authorization
           = new AuthenticationHeaderValue("Bearer", ConfigurationSettings.Token);

        HttpResponseMessage response = await httpClient.SendAsync(request);
        string content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);
    }
}

class Transaction
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}
