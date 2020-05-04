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
        public void Parse_lemonde_rss()
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
        public void Parse_marianne_rss()
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

        [Fact]
        public void Parse_liberation_rss()
        {
            var parser = new RssParser();

            var rssContent = parser.Parse(File.OpenRead("./rss_liberation.xml"));

            rssContent.Items.Should().HaveCount(50);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new RssItem
                {
                    Id = "tag:www.liberation.fr,2020-03-04:/1787260",
                    Link = "https://www.liberation.fr/planete/2020/05/04/direct-suivez-les-dernieres-informations-sur-la-pandemie-de-covid-19-en-france-et-dans-le-monde_1787260?xtor=rss-450",
                    Title = "Direct - Education, aide aux jeunes, entreprises... Edouard Philippe précise la stratégie de déconfinement",
                    Summary = "Pour le philosophe Abdennour Bidar, en voulant sauver la vie, nous l’avons dans le même temps coupée de tous les liens qui la nourrissent, vidée de toutes les significations qui la font grandir.",
                    PublishDate = new DateTimeOffset(2020, 5, 4, 16, 55, 44, TimeSpan.FromHours(2)),
                    ImageUrl = "https://medias.liberation.fr/photo/1311179-outbreak-of-the-coronavirus-disease-covid-19-in-paris.jpg?modified_at=1588572140&ratio_x=03&ratio_y=02&width=150"
                });
        }
    }
}