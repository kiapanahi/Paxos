using Paxos.Core.Contracts;

namespace Paxos.Core.Tests
{
    internal class SampleProposer : Proposer<string>
    {
        public SampleProposer(string identifier) : base(identifier, SampleAcceptorCollectionProvider.Instance, new EpochProposerNumberGenerator())
        {
        }
    }
}
