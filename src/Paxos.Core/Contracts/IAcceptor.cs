using System.Threading;
using System.Threading.Tasks;

namespace Paxos.Core.Contracts
{
    public interface IAcceptor<T>
    {
        Task<PrepareResponse> ReceivePrepareRequestAsync(PrepareRequest request);
        Task ReceiveAcceptRequestAsync(AcceptRequest<T> request);
    }
}
