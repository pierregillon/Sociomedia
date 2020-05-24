using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.Core.Application;
using Sociomedia.Medias.Domain;

namespace Sociomedia.Medias.Application.Commands.EditMedia
{
    public class EditMediaCommandHandler : ICommandHandler<EditMediaCommand>
    {
        private readonly IRepository _repository;

        public EditMediaCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(EditMediaCommand command)
        {
            var media = await _repository.Get<Media>(command.MediaId);

            media.Edit(command.Name, command.ImageUrl, command.PoliticalOrientation);
            media.UpdateFeeds(command.Feeds);

            await _repository.Save(media);
        }
    }
}