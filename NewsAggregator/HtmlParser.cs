using HtmlAgilityPack;

namespace NewsAggregator
{
    public class HtmlParser
    {
        public string FindArticle(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var articleNode = FindNode(doc.DocumentNode, "article");
            return articleNode != null ? articleNode.OuterHtml : html;
        }

        private static HtmlNode FindNode(HtmlNode node, string name)
        {
            if (node.Name == name) {
                return node;
            }

            foreach (var childNode in node.ChildNodes) {
                var result = FindNode(childNode, name);
                if (result != null) {
                    return result;
                }
            }

            return null;
        }
    }
}