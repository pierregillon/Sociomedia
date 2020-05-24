* try to reimport "leparisien"
* Logging : indent logs automatically

# Front
* Infinite scroll
* Add an article viewer to show article content without living website
* Add query and queryhandler to reduce coupling for ui query
* display articles count
* if no photo, display media photo
* keywords search can be easily cleared like tag (with an x)

# FeedAggregator 
* manual reset event to block aggregation if eventstore unreachable. (when live mode enabled)
* PublishDate vs UpdateDate
* add author
* change image url only if not null
* certain articles have no description, check rss feed
* check agoravox rss : no image url
* check card image for euronews invalid image.
* remove feed update date
* add media information
  => https://fr.wikipedia.org/wiki/Presse_quotidienne_nationale_fran%C3%A7aise
* remove feed articles without date
* Convert date fr to utc with timezone doesn't work on linux
  => https://dejanstojanovic.net/aspnet/2018/july/differences-in-time-zones-in-net-core-on-windows-and-linux-host-os/
* Update article when title or summary changed a little bit : introduce % accuracy

# ProjectionSynchronizer
* use transaction when multiple insertion in sql db (article + keyword)

# Keywords
* include document title in the text used for keyword processing
  * more priority that content ?
* remove useless words.
  * keep only nouns ? 
* analyse word that starts with a capital letter : important words (location, personnality, ...)

# Themes
* display 3 main themes on article card

# Features ?
* Show indication about free / paid articles