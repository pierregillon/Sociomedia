using System;
using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.Domain.Medias;

namespace Sociomedia.Application.Commands.AddMedia
{
    public class AddMediaCommandHandler : ICommandHandler<AddMediaCommand, Guid>
    {
        private readonly IRepository _repository;

        public AddMediaCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(AddMediaCommand command)
        {
            var media = new Media(command.Name, command.ImageUrl, command.PoliticalOrientation);

            foreach (var feed in command.Feeds) {
                media.AddFeed(feed);
            }

            await _repository.Save(media);

            return media.Id;
        }
    }
}