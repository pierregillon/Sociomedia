[![Build status](https://ci.appveyor.com/api/projects/status/j9039qpys7s18fnq/branch/master?svg=true)](https://ci.appveyor.com/project/pierregillon/sociomedia/branch/master)

# What is Sociomedia ?
Sociomedia is a media aggregator and a community that challenges information by evaluating articles.

## Media aggregation example
![medias example](./medias_aggregation_example.png?raw=true)

# Current features
- [X] Add, edit and remove medias
- [X] Define one or more RSS / ATOM feeds per media
- [X] Centralize articles from several sources in the same web page
- [X] Import all new articles on the flow
- [X] Allow a user to search articles on a specific subjects (keywords search)

Show [**current version**](https://github.com/pierregillon/sociomedia/releases)

# Roadmap
## Features
- [ ] Show themes or trending that group articles by same subject
- [ ] Display article content inside Sociomedia app (iframe)
- [ ] Define more information about a media : founding, creation date, political orientation, ...
- [ ] Allow users to rank an article
- [ ] Order articles with highest rank
- [ ] Make medias edition for admin only

## Tech
- [ ] Docker for deployment

# Architecture
Sociomedia is composed of 3 packages :
- **FeedAggregator** : aggregates periodicly all articles from specified rss / atom sources
- **ProjectionsSynchronizer** : Transform events to a read projection that is inserted in a sql database
- **Front** : Website that exposes user features

## Schema
![architecture](./architecture.png?raw=true)

## Databases
* Write database : [Event store](https://eventstore.com/docs/)
* Read database : Sql server or Postgresql

# Development
Let's talk here about technical details. You might be interested of this section if you want to run the code on your machine.

## How the application is built ?
The api is built following architecture patterns : 
- [Command Query Response Segregation (CQRS)](https://www.martinfowler.com/bliki/CQRS.html)
- [Domain Driven Design (DDD)](https://domainlanguage.com/ddd/)
- [Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html)

## Languages and technologies
Projects is written is C#, HTML, JS, CSS.
- Backend applications are .net core 3.1 console
- Frontend is a [**Blazor app**](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)

## Main libraries
* [CQRSLite](https://github.com/gautema/CQRSlite) : light library for DDD and CQRS programming
* [EventStore.Client](https://eventstore.com/docs/) : client to EventStore DB (the stream-oriented database optimised for event sourcing)
* [Linq2Db](https://github.com/linq2db/linq2db) : Linq to database provider, optimized in sql request building.

## Installing & Executing
0. Make sure .NET Core SDK is installed on your environment (dotnet command line tool)

1. Install dependencies
```
dotnet restore
```

2. Build
```
dotnet build
```

3. Run
```
dotnet run --project /src/Sociomedia.Front/Sociomedia.Front.csproj
dotnet run --project /src/Sociomedia.ProjectionSynchronizer/Sociomedia.ProjectionSynchronizer.csproj
dotnet run --project /src/Sociomedia.FeedAggregator/Sociomedia.FeedAggregator.csproj
```
## Running the tests
```
dotnet test
```
The tests run with xUnit.

## Running the tests
Tests are written following Behaviour Driven Development (BDD) with xUnit and NSubstitute (Mocks).

# Versioning
The project use [SemVer](http://semver.org/) for versioning. For the versions available, see [the tags on this repository](https://github.com/pierregillon/sociomedia/releases).

# License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
