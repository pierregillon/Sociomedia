* include document title in the text used for keyword processing
  * more priority that content ?
* remove useless words.
  * keep only nouns ? 
* analyse word that starts with a capital letter : important words (location, personnality, ...)


* continuous integration
* database configuration access (event store + sql db)
* rss / atom ressources creation
* projection synchronizer : infinite loop
* feed aggregator : infinite loop.
* sanitize image url from width/height format parameters
    => https://medias.liberation.fr/photo/1311428-emmanuel-macron-dans-une-ecole-des-yvelines-le-5-mai-2020-a-poissy.jpg?modified_at=1588686002&ratio_x=03&ratio_y=02&width=150
* Clear logging from EventPublisher decorator
* add tests on FeedReader when url unreachable
* fix add/remove rss/atom feed url to existing media
* ArticleImported => check summary format, it should not be html !
* Better management when article in unreachable : improve ArticleNotFound ?
* Be sure to not introduce article duplication : use article external id and store it
* Show indication about free / paid articles
* Paging for articles
* Display media details and its articles when clicking on a media in the media list or the logo in the articles list
* Add an article viewer to show article content without living website
* Removing articles when removing media
* Simplify article import : take all articles from feed and compare with ones existing
* Add query and queryhandler to reduce coupling for ui query

* global catch

# FeedAggregator 
* manual reset event to block aggregation if eventstore unreachable.
* PublishDate vs UpdateDate
* add author