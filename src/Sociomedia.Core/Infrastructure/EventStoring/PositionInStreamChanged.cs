using System.Threading.Tasks;

namespace Sociomedia.Core.Infrastructure.EventStoring
{
    public delegate Task PositionInStreamChanged(long position);
}