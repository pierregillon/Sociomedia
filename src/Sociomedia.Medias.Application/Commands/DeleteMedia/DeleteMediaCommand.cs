using System;
using Sociomedia.Core.Application;

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