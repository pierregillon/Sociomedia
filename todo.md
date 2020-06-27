* try to reimport "leparisien"
* test if an article is present in 2 rss feeds (lemonde)
* takerigth 100 max on json objet to avoid console to blow
* add black list keyword for front to avoid
* manage redondancy keywords : "macron", "emmanuel", "emmanuel macron"
* ArticleUpdated => update publishDate
* refacto ProjectionsBootstrapper
* remove date from keywords
* black list "mois, jour, bien, abonnés, offre,"
* warning: prod base, update KeywordCount column
* log when theme processing time > 1s
* keyword count in article table
* do not create theme if keyword intersection > 10 keywords

# Tech
* display current assembly version for all programs on execution (FeedAggregator, ProjectionSynchronizer, Front)
* Docker
* Logging : indent logs automatically

# Media domain
* add media information
  => https://fr.wikipedia.org/wiki/Presse_quotidienne_nationale_fran%C3%A7aise
* define custom css class to extract automatically image from online article

# Article domain
* add author
* certain articles have no description, check rss feed
* check agoravox rss : no image url
* check card image for euronews invalid image.
* Convert date fr to utc with timezone doesn't work on linux
  => https://dejanstojanovic.net/aspnet/2018/july/differences-in-time-zones-in-net-core-on-windows-and-linux-host-os/
  => add linux CI
* Update article not only on different publish date but also when title or summary changed a little bit : introduce % accuracy
* Show indication about free / paid articles
* If date is in future (rare but happend) filter it in front query : Date < now 

# Theme domain
* display 3 main themes on article card

# Front
* Infinite scroll
* Add an article viewer to show article content without living website
* Add query and queryhandler to reduce coupling for ui query
* if no photo, display media photo
* keywords search can be easily cleared like tag (with an x)
