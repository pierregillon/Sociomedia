using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewsAggregator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var urls = new[] {
                "https://www.lemonde.fr/international/article/2020/02/22/coronavirus-le-point-sur-l-epidemie_6030475_3210.html",
                "https://www.liberation.fr/planete/2020/02/23/coronavirus-l-epidemie-frappe-l-italie-s-enferme_1779385"
            };

            var factory = new ArticleFactory();
            var articles = new List<Article>();
            foreach (var url in urls) {
                var html = await Download(url);
                articles.Add(factory.Build(html));
            }
            foreach (var keyword in articles.First().Keywords.Intersect(articles.Last().Keywords)) {
                Console.WriteLine(keyword);
            }
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