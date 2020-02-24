using System.Collections.Generic;
using System.Linq;

namespace NewsAggregator
{
    public class ThemeBuilder
    {
        public ThemeBuilder()
        {
        }

        public IEnumerable<Theme> Build(IEnumerable<Article> articles)
        {
            List<Theme> themes = new List<Theme>();
            Dictionary<Article, List<Article>> comparison = new Dictionary<Article, List<Article>>();

            foreach (Article article1 in articles) {
                foreach (Article article2 in articles) {
                    if (article1 != article2) {
                        if (comparison.ContainsKey(article2) && comparison[article2]?.Contains(article1) == true) {
                            continue;
                        }
                        Keyword[] keywordIntersection = article1.Keywords.Intersect(article2.Keywords).ToArray();
                        if (keywordIntersection.Any()) {
                            var existing = themes.FirstOrDefault(theme => keywordIntersection.All(keyword => theme.Keywords.Contains(keyword)));
                            if(existing == null) {
                                themes.Add(new Theme(keywordIntersection, new[] { article1, article2 }));
                            }
                            else {
                                existing.AddArticle(article2);
                            }
                        }
                        if (comparison.ContainsKey(article1) == false) {
                            comparison.Add(article1, new List<Article>());
                        }
                        comparison[article1].Add(article2);
                    }
                }
            }

            return themes;
        }
    }
}