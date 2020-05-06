using System.Threading.Tasks;
using CQRSlite.Domain;
using Sociomedia.FeedAggregator.Domain.Medias;

namespace Sociomedia.FeedAggregator.Application.Commands.AddMedia
{
    public class AddMediaCommandHandler : ICommandHandler<AddMediaCommand>
    {
        private readonly IRepository _repository;

        public AddMediaCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(AddMediaCommand command)
        {
            var media = new Media(command.Name, command.ImageUrl, command.PoliticalOrientation);

            await _repository.Save(media);
        }
    }
}