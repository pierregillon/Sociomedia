* Removing articles when removing media

# Front
* Infinite scroll
* Add an article viewer to show article content without living website
* Add query and queryhandler to reduce coupling for ui query
* display articles count

# FeedAggregator 
* manual reset event to block aggregation if eventstore unreachable.
* PublishDate vs UpdateDate
* add author
* change image url only if not null
* certain articles have no description, check rss feed
* check agoravox rss : no image url
* check card image for euronews invalid image.
* remove feed update date

# Keywords
* include document title in the text used for keyword processing
  * more priority that content ?
* remove useless words.
  * keep only nouns ? 
* analyse word that starts with a capital letter : important words (location, personnality, ...)

# Features ?
* Show indication about free / paid articles