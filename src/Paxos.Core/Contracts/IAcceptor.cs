using System.Threading.Tasks;

namespace Paxos.Core.Contracts
{
    public interface IAcceptor<T>
    {
        Task<PrepareResponse> ReceivePrepareRequestAsync(PrepareRequest<T> request);
        Task ReceiveAcceptRequestAsync(AcceptRequest<T> request);
    }
}
