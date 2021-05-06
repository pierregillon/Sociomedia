﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Sociomedia.Core.Application;
using Sociomedia.Core.Domain;
using Sociomedia.ReadModel.DataAccess;
using Sociomedia.Themes.Domain;

namespace Sociomedia.ProjectionSynchronizer.Application.EventListeners
{
    public class ThemeTableSynchronizer :
        IEventListener<ThemeAdded>,
        IEventListener<ArticleAddedToTheme>,
        IEventListener<ThemeKeywordsUpdated>
    {
        private readonly DbConnectionReadModel _dbConnection;

        public ThemeTableSynchronizer(DbConnectionReadModel dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task On(ThemeAdded @event)
        {
            await _dbConnection.Themes
                .Value(x => x.Id, @event.Id)
                .Value(x => x.Name, BuildName(@event.Keywords))
                .Value(x => x.FullKeywords, JoinAllKeywords(@event.Keywords))
                .Value(x => x.KeywordCount, @event.Keywords.Count)
                .Value(x => x.OccurencePerKeywordPerArticle, (double)@event.Keywords.Sum(x => x.Occurence) / (@event.Keywords.Count * @event.Articles.Count))
                .InsertAsync();

            foreach (var articleId in @event.Articles) {
                await _dbConnection.ThemedArticles
                    .Value(x => x.ThemeId, @event.Id)
                    .Value(x => x.ArticleId, articleId)
                    .InsertAsync();
            }
        }

        public async Task On(ArticleAddedToTheme @event)
        {
            await _dbConnection.ThemedArticles
                .Value(x => x.ThemeId, @event.Id)
                .Value(x => x.ArticleId, @event.ArticleId)
                .InsertAsync();
        }

        public async Task On(ThemeKeywordsUpdated @event)
        {
            var query =
                from themedArticle in _dbConnection.ThemedArticles
                where themedArticle.ThemeId == @event.Id
                select 1;


            await _dbConnection.Themes
                .Where(x => x.Id == @event.Id)
                .Set(x => x.Name, BuildName(@event.Keywords))
                .Set(x => x.FullKeywords, JoinAllKeywords(@event.Keywords))
                .Set(x => x.KeywordCount, @event.Keywords.Count)
                .Set(x => x.OccurencePerKeywordPerArticle, (double) @event.Keywords.Sum(x => x.Occurence) / (@event.Keywords.Count * query.Count()))
                .UpdateAsync();
        }

        private static string BuildName(IReadOnlyCollection<Keyword> keywords)
        {
            return keywords
                .Pipe(RemoveSingleWordsContainedInComposedWords)
                .OrderByDescending(x => x.Occurence)
                .Select(x => x.Value.FirstLetterUpperForEachWords())
                .Take(4)
                .Pipe(x => string.Join(", ", x));
        }

        private static IEnumerable<Keyword> RemoveSingleWordsContainedInComposedWords(IReadOnlyCollection<Keyword> keywords)
        {
            var composedKeywords = keywords
                .Where(x => x.IsComposed)
                .ToArray();

            if (!composedKeywords.Any()) {
                foreach (var keyword in keywords) {
                    yield return keyword;
                }
            }
            else {
                foreach (var composedKeyword in composedKeywords) {
                    foreach (var keyword in keywords) {
                        if (composedKeyword.Equals(keyword)) {
                            yield return composedKeyword;
                        }
                        else if (!composedKeyword.Contains(keyword)) {
                            yield return keyword;
                        }
                    }
                }
            }
        }


        private static string JoinAllKeywords(IEnumerable<Keyword> keywords)
        {
            return keywords.Pipe(x => string.Join(" ; ", x));
        }
    }
}