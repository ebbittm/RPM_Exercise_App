using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace RPM_Exercise.Clients
{
    public interface IApiClient
    {
        public Task<T> Get<T>(string route);
    }

    public class ApiClient : IApiClient
    {
        private readonly string EIA_ENDPOINT = "https://api.eia.gov/v2/";
        //petroleum/pri/gnd/data/?frequency=weekly&data[0]=value&facets[series][]=EMD_EPD2D_PTE_NUS_DPG&sort[0][column]=period&sort[0][direction]=desc&offset=0&length=5000&api_key=EthXWE6eUTrBEJ1uTpNCqbL4NjghRxaC2R5tw1b2";



        public async Task<T?> Get<T>(string route)
        {
            var httpClient = GetClient();
            HttpResponseMessage response = await httpClient.GetAsync(route);
            response.EnsureSuccessStatusCode();
            Stream responseBody = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<T>(responseBody);
        }

        private HttpClient GetClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            HttpClient httpClient = new(handler) { BaseAddress = new Uri($"{EIA_ENDPOINT}") };

            return httpClient;
        }
    }
}
