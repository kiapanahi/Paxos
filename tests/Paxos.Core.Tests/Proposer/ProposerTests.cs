using System.Threading.Tasks;
using Xunit;

namespace Paxos.Core.Tests.Proposer
{

    public class ProposerTests
    {
        internal class LongRunningAcceptor : Acceptor<string>
        {
            private readonly int _delayMilliseconds;

            public LongRunningAcceptor(int delayMilliseconds): base("long-running-acceptor")
            {
                _delayMilliseconds = delayMilliseconds;
            }
            public override async Task<PrepareResponse> ReceivePrepareRequestAsync(PrepareRequest request)
            {
                await Task.Delay(_delayMilliseconds);
                return new PrepareResponse(true, new Proposal<string>(1337, "dummy-proposal"));
            }
        }

        [Theory]
        [InlineData(500, true, 1337, "dummy-proposal")]
        [InlineData(0, true, 1337, "dummy-proposal")]
        [InlineData(4_800, true, 1337, "dummy-proposal")]
        [InlineData(30_000, false, null, null)]
        [InlineData(5_000, false, null, null)]
        public async Task SendPrepareRequestToAccceptor_RespectsTimeout(int acceptorDelayMilliseconds,
                                 bool expectedPromise,
                                 long? proposalNumber,
                                 string? proposalValue)
        {
            var proposer = new SampleProposer("sample-proposer");

            var prepareRequestResponse = await proposer.SendPrepareRequestToAcceptor(new LongRunningAcceptor(acceptorDelayMilliseconds));

            Assert.NotNull(prepareRequestResponse);
            Assert.Equal(expectedPromise, prepareRequestResponse.Promised);
            if (expectedPromise is false)
            {
                Assert.Null(prepareRequestResponse.AcceptedProposal);
            }
            else
            {
                Assert.NotNull(prepareRequestResponse.AcceptedProposal);
                
                var proposal = prepareRequestResponse.AcceptedProposal!.As<Proposal<string>>();

                Assert.Equal(proposalNumber, proposal!.Number);
                Assert.Equal(proposalValue, proposal!.Value);
            }
        }
    }
}
