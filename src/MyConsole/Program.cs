using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace MyConsole {
    public class MyService {

        private readonly IHttpClientFactory _factory;

        public MyService(IHttpClientFactory factory) {
            _factory = factory;
        }

        public async Task Get() {
            var client = _factory.CreateClient("aaa");
            var rs = await client.GetAsync("https://localhost:5001/weatherForecast");

            Console.WriteLine(rs.StatusCode);
        }
    }

    class Program {
        static async Task Main(string[] args) {

            // ServicePointManager.ServerCertificateValidationCallback =
            // delegate (
            //     object s,
            //     X509Certificate certificate,
            //     X509Chain chain,
            //     SslPolicyErrors sslPolicyErrors
            // ) {
            //     return true;
            // };

            var collection = new ServiceCollection();

            collection.AddHttpClient("aaa").ConfigurePrimaryHttpMessageHandler(() => {
                var handler = new HttpClientHandler {
                    // ServerCertificateCustomValidationCallback = delegate { return true; },
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                return handler;
            });

            collection.AddSingleton<MyService>();

            var provider = collection.BuildServiceProvider();
            var myService = provider.GetService<MyService>();

            await myService.Get();
        }
    }
}
