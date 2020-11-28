using System;
using System.Threading;
using System.Threading.Tasks;
using Paxos.Core.Contracts;

namespace Paxos.Core
{
    public class Acceptor<T> : IAcceptor<T>
    {
        private readonly AsyncLocal<Proposal> _proposal;
        public Acceptor(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            Identifier = identifier;
            _proposal = new AsyncLocal<Proposal>
            {
                Value = new Proposal(long.MinValue)
            };
        }
        public string Identifier { get; }

        public Proposal AcceptedProposal => _proposal.Value!;
        public long AcceptedProposalNumber => AcceptedProposal.Number;


        public Task ReceiveAcceptRequestAsync(AcceptRequest<T> request) => throw new NotImplementedException();
        public virtual Task<PrepareResponse> ReceivePrepareRequestAsync(PrepareRequest request)
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
