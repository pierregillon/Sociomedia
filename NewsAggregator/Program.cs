using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewsAggregator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var url = "https://www.lemonde.fr/international/article/2020/02/22/coronavirus-le-point-sur-l-epidemie_6030475_3210.html";

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