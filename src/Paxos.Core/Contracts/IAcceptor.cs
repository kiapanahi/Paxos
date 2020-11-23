using System.Threading.Tasks;

namespace Paxos.Core.Contracts
{
    public interface IAcceptor
    {
        Task<PrepareResponse> ReceivePrepareRequest(PrepareRequest request);
        Task ReceiveAcceptRequest<T>(AcceptRequest<T> request) where T : Proposal<T>;
    }
}
