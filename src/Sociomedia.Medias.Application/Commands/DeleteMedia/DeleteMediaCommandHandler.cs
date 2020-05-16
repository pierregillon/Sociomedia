using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.Application.Application;
using Sociomedia.Medias.Domain;

namespace Sociomedia.Medias.Application.Commands.DeleteMedia {
    public class DeleteMediaCommandHandler : ICommandHandler<DeleteMediaCommand>
    {
        private readonly IRepository _repository;

        public DeleteMediaCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteMediaCommand command)
        {
            var media = await _repository.Get<Media>(command.MediaId);

            media.Delete();

            await _repository.Save(media);
        }
    }
}