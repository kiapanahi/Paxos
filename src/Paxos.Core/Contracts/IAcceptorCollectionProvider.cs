using System.Threading.Tasks;

namespace Paxos.Core.Contracts
{
    public interface IAcceptorCollectionProvider<T>
    {
        public Task<IAcceptor<T>[]> GetAcceptorsAsync();
    }
}
