using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Sociomedia.FeedAggregator.Domain;
using Sociomedia.FeedAggregator.Infrastructure;
using Sociomedia.Infrastructure;
using Xunit;

namespace FeedAggregator.Tests
{
    public class RssParserTests
    {
        private FeedParser parser;

        public RssParserTests()
        {
            parser = new FeedParser(new HtmlParser());
        }

        [Fact]
        public void Parse_lemonde_rss()
        {
            var rssContent = parser.Parse(File.OpenRead("./Resources/rss_lemonde.xml"));

            rssContent.Items.Should().HaveCount(20);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new FeedItem {
                    Id = "https://www.lemonde.fr/idees/article/2020/05/03/coronavirus-il-a-suffi-qu-un-hote-s-invite-et-brise-l-equilibre-d-un-systeme-qui-se-croyait-infaillible_6038499_3232.html",
                    Link = "https://www.lemonde.fr/idees/article/2020/05/03/coronavirus-il-a-suffi-qu-un-hote-s-invite-et-brise-l-equilibre-d-un-systeme-qui-se-croyait-infaillible_6038499_3232.html",
                    Title = "Coronavirus : « Il a suffi qu’un hôte s’invite et brise l’équilibre d’un système qui se croyait infaillible »",
                    Summary = "Dans une tribune au « Monde », le syndicaliste Hubert Bouchet retrace la singulière trajectoire du Covid-19 qui révèle les défaillances des organisations économiques et sociales à l’échelle planétaire.",
                    PublishDate = new DateTimeOffset(2020, 5, 3, 10, 0, 15, TimeSpan.FromHours(2)),
                    ImageUrl = "https://img.lemde.fr/2020/04/30/155/0/5605/2802/644/322/60/0/e090270_VFB1FBdC80hLCjU9K5_xwsje.jpg"
                });
        }

        [Fact]
        public void Parse_marianne_rss()
        {
            var rssContent = parser.Parse(File.OpenRead("./Resources/rss_marianne.xml"));

            rssContent.Items.Should().HaveCount(20);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new FeedItem {
                    Id = "https://www.marianne.net/politique/semiologie-de-didier-raoult-le-look-emmerdeur",
                    Link = "https://www.marianne.net/politique/semiologie-de-didier-raoult-le-look-emmerdeur",
                    Title = "Sémiologie de Didier Raoult : le look emmerdeur",
                    Summary = "BIIIIP ! Ennuis à l'horizon ! Certains signaux vestimentaires en disent plus qu'une fiche LinkedIn ou même qu'une fréquentation amicale assidue. Ainsi les chantres du poil à gratter, les génies de l'insoumission, les pas “corporate”, qui “ne jouent pas le jeu” s'annoncent-ils par une sémiologie bien spéciale.",
                    PublishDate = new DateTimeOffset(2020, 5, 3, 11, 30, 0, 0, TimeSpan.FromHours(2)),
                    ImageUrl = null
                });
        }

        [Fact]
        public void Parse_liberation_rss()
        {
            var rssContent = parser.Parse(File.OpenRead("./Resources/rss_liberation.xml"));

            rssContent.Items.Should().HaveCount(50);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new FeedItem {
                    Id = "tag:www.liberation.fr,2020-03-04:/1787260",
                    Link = "https://www.liberation.fr/planete/2020/05/04/direct-suivez-les-dernieres-informations-sur-la-pandemie-de-covid-19-en-france-et-dans-le-monde_1787260?xtor=rss-450",
                    Title = "Direct - Education, aide aux jeunes, entreprises... Edouard Philippe précise la stratégie de déconfinement",
                    Summary = "Pour le philosophe Abdennour Bidar, en voulant sauver la vie, nous l’avons dans le même temps coupée de tous les liens qui la nourrissent, vidée de toutes les significations qui la font grandir.",
                    PublishDate = new DateTimeOffset(2020, 5, 4, 16, 55, 44, TimeSpan.FromHours(2)),
                    ImageUrl = "https://medias.liberation.fr/photo/1311179-outbreak-of-the-coronavirus-disease-covid-19-in-paris.jpg"
                });
        }

        [Fact]
        public void Parse_franceTVInfo_rss()
        {
            var rssContent = parser.Parse(File.OpenRead("./Resources/rss_francetvinfo.xml"));

            rssContent.Items.Should().HaveCount(20);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new FeedItem {
                    Id = "https://www.francetvinfo.fr/sante/maladie/coronavirus/les-detenus-sont-inquiets-surtout-pour-leur-famille-a-mulhouse-les-surveillants-de-prison-doivent-proteger-et-rassurer-face-au-coronavirus_3948277.html#xtor=RSS-3-[france]",
                    Link = "https://www.francetvinfo.fr/sante/maladie/coronavirus/les-detenus-sont-inquiets-surtout-pour-leur-famille-a-mulhouse-les-surveillants-de-prison-doivent-proteger-et-rassurer-face-au-coronavirus_3948277.html#xtor=RSS-3-[france]",
                    Title = "\"Les détenus sont inquiets, surtout pour leur famille\" : à Mulhouse, les surveillants de prison doivent protéger et rassurer face au coronavirus",
                    Summary = "Il a fallu réorganiser le travail pour empêcher le virus de pénétrer les murs des prisons. Christopher Pécoraro travaille à la maison d’arrêt de Mulhouse, au cœur d’un département parmi les plus touchés de France.",
                    PublishDate = new DateTimeOffset(2020, 5, 4, 17, 50, 19, TimeSpan.FromHours(2)),
                    ImageUrl = "https://www.francetvinfo.fr/image/75rzejnyj-19ca/600/337/21454101.jpg"
                });
        }

        [Fact]
        public void Parse_LHumanite_rss()
        {
            var rssContent = parser.Parse(File.OpenRead("./Resources/rss_lhumanite.xml"));

            rssContent.Items.Should().HaveCount(10);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new FeedItem
                {
                    Id = "688858",
                    Link = "https://www.humanite.fr/il-y-urgence-le-billet-du-dr-christophe-prudhomme-charite-688858#xtor=RSS-1",
                    Title = "Il y a urgence ! Le billet du Dr Christophe Prudhomme. Charité",
                    Summary = "Christophe Prudhomme est médecin au Samu 93.",
                    PublishDate = new DateTimeOffset(2020, 5, 8, 09, 05, 49, TimeSpan.FromHours(0)),
                    ImageUrl = "https://www.humanite.fr/sites/default/files/styles/1048x350/public/images/prudhommefacelly_0.jpg?itok=9qa6R2Hc"
                });
        }

        [Fact]
        public void Parse_UsineNouvelle_rss()
        {
            var rssContent = parser.Parse(File.OpenRead("./Resources/rss_usinenouvelle.xml"));

            rssContent.Items.Should().HaveCount(50);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new FeedItem
                {
                    Id = "https://www.usinenouvelle.com/article/deficit-public-pour-tous-chocs-personnalises.N960076",
                    Link = "https://www.usinenouvelle.com/article/deficit-public-pour-tous-chocs-personnalises.N960076",
                    Title = "Déficit public pour tous, chocs personnalisés",
                    Summary = "La réponse des États européens à la crise ne s’est pas fait attendre. Les dépenses d’urgence, pour assurer les coûts sanitaires, préserver les entreprises et contenir le chômage, ont été massives. Les prévisions livrées par Goldman Sachs intègrent les chiffrages actuels des gouvernements de […] Lire l'article",
                    PublishDate = new DateTimeOffset(2020, 5, 8, 13, 0, 0, TimeSpan.FromHours(2)),
                    ImageUrl = "https://www.usinenouvelle.com/mediatheque/1/5/4/000867451_image_256x170.JPG"
                });
        }

        [Fact]
        public void Parse_Figaro_rss()
        {
            var rssContent = parser.Parse(File.OpenRead("./Resources/rss_figaro.xml"));

            rssContent.Items.Should().HaveCount(20);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new FeedItem
                {
                    Id = "https://www.lefigaro.fr/placement/comment-le-covid-pourrait-redessiner-les-contours-du-marche-immobilier-20200507",
                    Link = "https://www.lefigaro.fr/placement/comment-le-covid-pourrait-redessiner-les-contours-du-marche-immobilier-20200507",
                    Title = "Comment le Covid pourrait redessiner les contours du marché immobilier",
                    Summary = "DÉCRYPTAGE - L’absence quasi-totale de transactions rend difficiles les pronostics sur l’évolution des prix de la pierre, mais les professionnels notent déjà quelques tendances fortes. Qui s’accentueront si le confinement se prolonge.",
                    PublishDate = new DateTimeOffset(2020, 5, 7, 23, 11, 23, TimeSpan.FromHours(2)),
                    ImageUrl = null
                });
        }


        [Fact]

        public void Parse_lemediapresse_rss()
        {
            var rssContent = parser.Parse(File.OpenRead("./Resources/rss_lemediapresse.xml"));

            rssContent.Items.Should().HaveCount(10);

            rssContent.Items
                .First()
                .Should()
                .BeEquivalentTo(new FeedItem {
                    Id = "https://lemediapresse.fr/?p=10354",
                    Link = "https://lemediapresse.fr/social/la-reforme-des-retraites-point-dorgue-du-profond-malaise-enseignant/",
                    Title = "La réforme des retraites, point d’orgue du profond malaise enseignant",
                    Summary = "Directement concerné par la suppression des régimes spéciaux, le corps enseignant a été l’un des premiers à embrasser le mouvement contre la réforme des retraites. Mais pour les jeunes profs en début de carrière, cette mobilisation cache une colère plus profonde. Entre larmes, colère et combativité, rencontre avec deux jeunes professeures de banlieue parisienne. […]",
                    PublishDate = new DateTimeOffset(2020, 1, 3, 9, 40, 50, TimeSpan.FromHours(0)),
                    ImageUrl = "https://i1.wp.com/lemediapresse.fr/wp-content/uploads/2019/12/000_1L10W8.jpg?ssl=1"
                });
        }
    }
}