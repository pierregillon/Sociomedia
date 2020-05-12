* Removing articles when removing media [DOMAIN event ?]
* try to reimport "leparisien"

# Front
* Infinite scroll
* Add an article viewer to show article content without living website
* Add query and queryhandler to reduce coupling for ui query
* display articles count
* if no photo, display media photo

# FeedAggregator 
* manual reset event to block aggregation if eventstore unreachable.
* PublishDate vs UpdateDate
* add author
* change image url only if not null
* certain articles have no description, check rss feed
* check agoravox rss : no image url
* check card image for euronews invalid image.
* remove feed update date
* html decode title (monde diplomatique)
* Convert date fr to utc with timezone doesn't work on linux
  => https://dejanstojanovic.net/aspnet/2018/july/differences-in-time-zones-in-net-core-on-windows-and-linux-host-os/
* leparisien : no date in rss, but in url ! Try to extract date if possible from url
  => http://www.leparisien.fr/politique/rss.xml
  => http://www.leparisien.fr/elections/municipales/municipales-edouard-philippe-accelere-pour-le-second-tour-en-juin-12-05-2020-8315910.php#xtor=RSS-1481423633
* add media information
  => https://fr.wikipedia.org/wiki/Presse_quotidienne_nationale_fran%C3%A7aise

# ProjectionSynchronizer
* use transaction when multiple insertion in sql db (article + keyword)

# Keywords
* include document title in the text used for keyword processing
  * more priority that content ?
* remove useless words.
  * keep only nouns ? 
* analyse word that starts with a capital letter : important words (location, personnality, ...)

# Features ?
* Show indication about free / paid articles