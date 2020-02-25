using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using NewsAggregator.Themes;

namespace NewsAggregator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var urls = new[] {
                "https://www.lemonde.fr/international/article/2020/02/22/coronavirus-le-point-sur-l-epidemie_6030475_3210.html",
                "https://www.liberation.fr/planete/2020/02/23/coronavirus-l-epidemie-frappe-l-italie-s-enferme_1779385",
                "https://www.francetvinfo.fr/sante/maladie/coronavirus/coronavirus-covid-19-il-n-y-pas-d-epidemie-en-france-assure-le-ministre-de-la-sante_3838415.html",
                "https://www.20minutes.fr/sante/2725023-20200223-coronavirus-70-nouveaux-hopitaux-vont-etre-actives-faire-face-eventuelle-propagation-annonce-ministre-sante",
                "https://www.rtl.fr/actu/politique/coronavirus-oiivier-veran-annonce-que-70-hopitaux-supplementaires-vont-etre-actives-7800148130"
            };

            var factory = new ArticleFactory();
            var articles = new List<Article>();
            foreach (var url in urls) {
                var html = await Download(url);
                articles.Add(factory.Build(url, html));
            }

            var themeManager = new ThemeManager();
            foreach (var article in articles.Select(x => new ThemeArticle(x.Keywords.SelectMany(keyword => keyword.Words).ToArray()))) {
                themeManager.Add(article);
            }

            //foreach (var theme in themes) {
            //    Console.WriteLine("Theme : " + string.Concat(", ", theme.Keywords));
            //    Console.WriteLine(" - Articles :");
            //    foreach (var article in theme.Articles) {
            //        Console.WriteLine("  - " + article.Name);
            //    }
            //    Console.WriteLine("----------");
            //}
            Console.WriteLine("-> ended.");
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