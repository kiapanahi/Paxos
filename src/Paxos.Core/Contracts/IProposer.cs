using System.Threading.Tasks;

namespace Paxos.Core.Contracts
{
    public interface IProposer<T>
    {
        Task<PrepareResponse> SendPrepareRequestAsync();
        Task SendAcceptRequest(AcceptRequest<T> request);
    }
}
