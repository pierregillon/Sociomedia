using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NewsAggregator.Infrastructure.RSS;
using Xunit;

namespace NewsAggregator.Tests
{
    public class RssParserTests
    {
        [Fact]
        public void Parse_standard_rss()
        {
            var parser = new RssParser();

            var rssContent = parser.Parse(File.OpenRead("./rss_lemonde.xml"));

            rssContent.Items.Should().HaveCount(20);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new RssItem {
                    Id = "https://www.lemonde.fr/idees/article/2020/05/03/coronavirus-il-a-suffi-qu-un-hote-s-invite-et-brise-l-equilibre-d-un-systeme-qui-se-croyait-infaillible_6038499_3232.html",
                    Link = "https://www.lemonde.fr/idees/article/2020/05/03/coronavirus-il-a-suffi-qu-un-hote-s-invite-et-brise-l-equilibre-d-un-systeme-qui-se-croyait-infaillible_6038499_3232.html",
                    Title = "Coronavirus : « Il a suffi qu’un hôte s’invite et brise l’équilibre d’un système qui se croyait infaillible »",
                    Summary = "Dans une tribune au « Monde », le syndicaliste Hubert Bouchet retrace la singulière trajectoire du Covid-19 qui révèle les défaillances des organisations économiques et sociales à l’échelle planétaire.",
                    PublishDate = new DateTimeOffset(2020, 5, 3, 10, 0, 15, TimeSpan.FromHours(2)),
                    ImageUrl = "https://img.lemde.fr/2020/04/30/155/0/5605/2802/644/322/60/0/e090270_VFB1FBdC80hLCjU9K5_xwsje.jpg"
                });
        }

        [Fact]
        public void Parse_non_standard_rss_with_invalid_date_format()
        {
            var parser = new RssParser();

            var rssContent = parser.Parse(File.OpenRead("./rss_marianne.xml"));

            rssContent.Items.Should().HaveCount(20);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new RssItem
                {
                    Id = "https://www.marianne.net/politique/semiologie-de-didier-raoult-le-look-emmerdeur",
                    Link = "https://www.marianne.net/politique/semiologie-de-didier-raoult-le-look-emmerdeur",
                    Title = "Sémiologie de Didier Raoult : le look emmerdeur",
                    Summary = "BIIIIP ! Ennuis à l'horizon ! Certains signaux vestimentaires en disent plus qu'une fiche LinkedIn ou même qu'une fréquentation amicale assidue. Ainsi les chantres du poil à gratter, les génies de l'insoumission, les pas “corporate”, qui “ne jouent pas le jeu” s'annoncent-ils par une sémiologie bien spéciale. ",
                    PublishDate = new DateTimeOffset(2020, 5, 3, 11, 30, 0, TimeSpan.FromHours(2)),
                    ImageUrl = null
                });
        }
    }
}