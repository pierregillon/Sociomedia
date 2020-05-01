using System.IO;
using FluentAssertions;
using NewsAggregator.Infrastructure;
using Xunit;

namespace NewsAggregator.Tests
{
    public class HtmlParserTests
    {
        private readonly HtmlParser _htmlParser;

        public HtmlParserTests()
        {
            _htmlParser = new HtmlParser();
        }

        [Fact]
        public void Extract_article_content_from_html()
        {
            const string html = @"
                <html>
                    <body>
                    <p>text outside article</p>
                    <article>
                        <p class=""article__paragraph "">
                            Simple article content
                        </p>
                    </article>
                    <script>alert('some script')</script>
                </html>";

            var htmlArticle = _htmlParser.ExtractPlainTextArticleContent(html);

            htmlArticle
                .Should()
                .Be("Simple article content");
        }

        [Fact]
        public void Extract_real_content_from_html_article()
        {
            var html = File.ReadAllText("./article_example.html");

            var htmlArticle = _htmlParser.ExtractPlainTextArticleContent(html);

            htmlArticle
                .Should()
                .Be(@"Du personnel médical parmi des patients présentant des symptômes du coronavirus, à l’hôpital temporaire de Fangcai, à Wuhan, le 18 février. STR / AFP Deuxième décès en Italie, des foyers de la maladie qui se multiplient... L’épidémie de coronavirus poursuit sa progression à travers le monde. Elle touche désormais vingt-six pays et territoires en dehors de la Chine continentale et a fait une dizaine de morts. Deuxième mort en Italie Après un premier décès en Italie – celui d’un homme de 78 ans –, une deuxième personne testée positive au coronavirus est morte dans le pays, ont annoncé, samedi 22 février, les agences d’information. Il s’agit d’une Italienne qui était hospitalisée depuis une dizaine de jours et résidait en Lombardie. Ce sont les premiers Européens à succomber après avoir été infectés par le nouveau coronavirus. Le cap des 100 cas confirmés a été franchi dans le pays. « Dans les zones considérées comme des foyers [de l’épidémie de coronavirus], ni l’entrée ni la sortie ne sera autorisée sauf dérogation particulière », a déclaré le président du Conseil Giuseppe Conte, annonçant aussi la fermeture des entreprises et des écoles ainsi que l’annulation des évènements publics. Ainsi, trois matchs de football de série A, prévus dimanche, vont être reportés. Cette vague de contaminations avait déjà contraint, la veille, les autorités à prendre des mesures drastiques : les lieux publics ont été fermés vendredi pour une semaine dans onze villes du nord de l’Italie. Article réservé à nos abonnés Lire aussi Forte inquiétude en Italie face à l’épidémie due au coronavirus 123 nouveaux cas en Corée du Sud Le nombre de contaminations au coronavirus a atteint 556 cas en Corée du Sud, avec 123 nouveaux malades, ont annoncé les autorités locales dimanche. Une deuxième personne est morte de la maladie Covid-19, a ajouté le Centre coréen de contrôle et de prévention des maladies (KCDC) dans un communiqué. En tout, quatre personnes sont mortes. Parmi les nouveaux malades figure un employé d’une usine Samsung Electronics à Gumi, à 200 kilomètres au sud-est de Séoul, ce qui a amené le géant technologique à annoncer une suspension des activités sur ce site jusqu’à lundi. Le pays a rehaussé au plus « haut » son niveau d’alerte, a annoncé dimanche le président Moon Jae-in. L’épidémie est « à un tournant décisif », a-t-il ajouté à l’issue d’une réunion de son gouvernement sur ce sujet. L’un des deux principaux foyers de contamination du pays reste l’hôpital de Cheongdo, dans le sud du pays : 95 des dernières contaminations rapportées sont « liées », selon le KCDC, à des patients ou à du personnel de cet établissement, où les deux personnes décédées avaient été admises. Autre lieu particulièrement touché par le virus : l’Eglise de Shincheonji de Jésus, une secte chrétienne de la ville de Daegu, non loin de Cheongdo, dont plus de 200 membres ont été infectés.");
        }

        [Fact]
        public void Extract_all_content_if_no_article_content_found()
        {
            const string html = @"
                <html>
                    <body>
                    <p>text outside article</p>
                    <script>alert('some script')</script>
                </html>";

            var htmlArticle = _htmlParser.ExtractPlainTextArticleContent(html);

            htmlArticle
                .Should()
                .Be("text outside article alert('some script')");
        }
    }
}