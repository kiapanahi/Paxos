using System;
using System.Linq;
using System.Threading.Tasks;
using Paxos.Core.Contracts;

namespace Paxos.Core.Tests
{
    internal class SampleAcceptorCollectionProvider : IAcceptorCollectionProvider<string>
    {
        private readonly IAcceptor<string>[] _acceptors;
        public SampleAcceptorCollectionProvider()
        {
            _acceptors = Enumerable.Range(1, 20).Select(i => new SampleAcceptor($"sample-acceptor-{i:D2}")).ToArray();
        }

        private static readonly Lazy<IAcceptorCollectionProvider<string>> _instance = new Lazy<IAcceptorCollectionProvider<string>>(() => new SampleAcceptorCollectionProvider());
        public static IAcceptorCollectionProvider<string> Instance => _instance.Value;

        public Task<IAcceptor<string>[]> GetAcceptorsAsync() => Task.FromResult(_acceptors);

    }
}
