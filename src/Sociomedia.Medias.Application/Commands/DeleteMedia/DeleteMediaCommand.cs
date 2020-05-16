using System;
using Sociomedia.Application.Application;

namespace Sociomedia.Medias.Application.Commands.DeleteMedia
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