using System;
using System.Threading;
using System.Threading.Tasks;
using Paxos.Core.Contracts;

namespace Paxos.Core
{
    public abstract class Acceptor<T> : IAcceptor<T>
    {
        private readonly AsyncLocal<Proposal> _proposal;
        public Acceptor()
        {
            _proposal = new AsyncLocal<Proposal>
            {
                Value = new Proposal(long.MinValue)
            };
        }

        public Proposal AcceptedProposal => _proposal.Value!;
        public long AcceptedProposalNumber => AcceptedProposal.Number;

        public abstract string Identifier { get; }

        public Task ReceiveAcceptRequestAsync(AcceptRequest<T> request) => throw new NotImplementedException();
        public Task<PrepareResponse> ReceivePrepareRequestAsync(PrepareRequest<T> request)
        {
            if (request.Proposal.Number > AcceptedProposalNumber)
            {
                _proposal.Value = request.Proposal;
                return Task.FromResult(new PrepareResponse(true, AcceptedProposal));
            }

            return Task.FromResult(new PrepareResponse(false, AcceptedProposal));
        }


        public override string ToString() => $"{Identifier}: {AcceptedProposal}";
    }
}
