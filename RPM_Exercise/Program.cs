using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RPM_Exercise.Accessors;
using RPM_Exercise.Clients;
using RPM_Exercise.Entities;
using RPM_Exercise.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using RPM_Exercise.Managers;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2.HPack;
using Microsoft.EntityFrameworkCore;

namespace RPM_Exercise
{
    public class Program
    {
        static HttpClient client = new HttpClient();
        static Timer timer;

        static async Task<ResponseDto> GetPetroleumData(string path)
        {
            HttpResponseMessage response = await client.GetAsync(path);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            ResponseDto? responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseDto>(responseBody);

            return responseData;
            
        }

        static void Main(string[] args)
        {
            var host = GetHostBuilder().Build();

            IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var config = GetParameters();
            var delay = config.GetSection("Delay");

            int delayInt = 7;
            int.TryParse(delay.Value, out delayInt);

            StartDataThread(delayInt, provider);

            Console.ReadLine();
        }

        static void StartDataThread(int delayInDays, IServiceProvider provider)
        {
            client.BaseAddress = new Uri("https://api.eia.gov/v2/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            timer = new Timer(async state => await Run(provider), null, TimeSpan.Zero, new TimeSpan(delayInDays, 0, 0, 0));
        }

        static async Task Run(IServiceProvider hostProvider)
        {
            try
            {
                Console.WriteLine("Making weekly fetch for new data.");
                
                ResponseDto petroleumData = await GetPetroleumData("petroleum/pri/gnd/data/?frequency=weekly&data[0]=value&facets[series][]=EMD_EPD2D_PTE_NUS_DPG&sort[0][column]=period&sort[0][direction]=desc&offset=0&length=5000&api_key=EthXWE6eUTrBEJ1uTpNCqbL4NjghRxaC2R5tw1b2");

                Console.WriteLine("New petroleum data has been fetched.");

                var manager = hostProvider.GetService<PetroleumDataManager>();

                List<PetroleumDto> petroleumList = petroleumData.Response.Data;

                Console.WriteLine("Writing new petroleum data to the database.");
                await manager.CreatePetroleumData(petroleumList);

                Console.WriteLine("Deleting database data that is older than the specified days.");
                await manager.DeleteOldPetroleumData();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        private static IConfigurationSection GetParameters()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var val1 = builder.Build().GetSection("Parameters");

            return val1;
        }

        private static IHostBuilder GetHostBuilder()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(ac =>
           {
               ac.AddJsonFile("appsettings.json");
           })
                .ConfigureServices(s =>
                {
                    s.AddDbContext<PetroleumContext>(ServiceLifetime.Transient);
                    s.AddTransient<PetroleumAccessor>();
                    s.AddTransient<PetroleumDataManager>();
                });

            return host;
        }
    }
}