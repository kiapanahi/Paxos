using System.Threading.Tasks;

namespace Paxos.Core.Contracts
{
    public interface IProposer<T>
    {
        Task SendPrepareRequestAsync();
        Task SendAcceptRequestAsync(AcceptRequest<T> request);
    }
}
