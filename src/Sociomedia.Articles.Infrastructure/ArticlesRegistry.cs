using System;
using System.IO;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Feeds;
using Sociomedia.Articles.Domain.Keywords;
using Sociomedia.Core.Application;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Sociomedia.Articles.Infrastructure
{
    public class ArticlesRegistry : Registry
    {
        public ArticlesRegistry()
        {
            Scan(scanner => {
                scanner.Assembly("Sociomedia.Articles.Application");
                scanner.Convention<AllInterfacesConvention>();
                scanner.AddAllTypesOf(typeof(IEventListener<>));
                scanner.AddAllTypesOf(typeof(ICommandHandler<>));
            });

            For<FrenchKeywordDictionaryConfiguration>().Use(new FrenchKeywordDictionaryConfiguration(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./Dictionaries/french.csv"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./Dictionaries/french_black_list.txt")
            ));

            For<FrenchKeywordDictionary>().Use<FrenchKeywordDictionary>().Singleton();

            For<IKeywordDictionary>().Use(x => x.GetInstance<FrenchKeywordDictionary>());

            For<IFeedReader>().Use<FeedReader>();
            For<IFeedParser>().Use<FeedParser>();
            For<IHtmlParser>().Use<HtmlParser>();
            For<IWebPageDownloader>().Use<WebPageDownloader>();
        }

        private class AllInterfacesConvention : IRegistrationConvention
        {
            public void ScanTypes(TypeSet types, Registry services)
            {
                foreach (var type in types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)) {
                    foreach (var @interface in type.GetInterfaces()) {
                        services.For(@interface).Use(type);
                    }
                }
            }
        }
    }
}