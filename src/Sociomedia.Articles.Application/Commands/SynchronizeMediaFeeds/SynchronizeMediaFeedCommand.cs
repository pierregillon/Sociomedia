using System;
using Sociomedia.Core.Application;

namespace Sociomedia.Articles.Application.Commands.SynchronizeMediaFeeds
{
    public class SynchronizeMediaFeedCommand : ICommand
    {
        public Guid MediaId { get; }
        public string FeedUrl { get; }

        public SynchronizeMediaFeedCommand(Guid mediaId, string feedUrl)
        {
            MediaId = mediaId;
            FeedUrl = feedUrl;
        }
    }
}