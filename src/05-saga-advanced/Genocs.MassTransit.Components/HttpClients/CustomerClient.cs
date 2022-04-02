using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.HttpClients
{
    public class CustomerClient
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        private readonly JsonSerializerOptions _options;
        public CustomerClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;

            _options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        public async Task<Customer> GetCustomer(string customerNumber)
        {
            Customer customer = null;
            var uri = new Uri($"https://localhost:7115/Customers?customerNumber={customerNumber}");
            var response = await _client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                //res = await response.Content.ReadAsStringAsync();
                //Customer customer = JsonSerializer.Deserialize<Customer>(res, _options);
                //res = customer?.CustomerNumber;
                var contentStream = await response.Content.ReadAsStreamAsync();
                customer = await JsonSerializer.DeserializeAsync<Customer>(contentStream, _options);

            }
            return customer;
        }
    }
}
