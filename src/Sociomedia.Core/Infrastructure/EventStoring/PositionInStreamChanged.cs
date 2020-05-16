using System.Threading.Tasks;

namespace Sociomedia.Application.Infrastructure.EventStoring
{
    public delegate Task PositionInStreamChanged(long position);
}