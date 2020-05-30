* try to reimport "leparisien"
* test if an article is present in 2 rss feeds (lemonde)
* theme projection must be updated as soon theme event updated !
* theme domain logics process ocurrence, not in theme management => command vs events ?
* add article to theme => update theme keywords and occurence

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

# Theme domain
* display 3 main themes on article card

# Front
* Infinite scroll
* Add an article viewer to show article content without living website
* Add query and queryhandler to reduce coupling for ui query
* if no photo, display media photo
* keywords search can be easily cleared like tag (with an x)
