using System;

namespace Sociomedia.Application.Commands.DeleteMedia
{
    public class DeleteMediaCommand : ICommand
    {
        public Guid MediaId { get; }

        public DeleteMediaCommand(Guid mediaId)
        {
            MediaId = mediaId;
        }
    }
}