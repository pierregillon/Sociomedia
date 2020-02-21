using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewsAggregator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var url = "https://www.lefigaro.fr/sciences/2020/02/21/01008-20200221LIVWWW00001-en-direct-coronavirus-epidemie-chine-Wuhan-Branville-covid-19-rapatries.php";

            var html = await Download(url);

            var factory = new ArticleFactory();

            var article = factory.Build(html);

            Console.ReadKey();
        }

        private static async Task<string> Download(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }
}