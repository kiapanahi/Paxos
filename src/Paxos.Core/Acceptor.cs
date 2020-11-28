using System;
using System.Threading;
using System.Threading.Tasks;
using Paxos.Core.Contracts;

namespace Paxos.Core
{
    public class Acceptor<T> : IAcceptor<T>
    {
        private readonly AsyncLocal<Proposal> _promisedProposal;
        public Acceptor(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            Identifier = identifier;
            _promisedProposal = new AsyncLocal<Proposal>
            {
                Value = new Proposal(long.MinValue)
            };
        }
        public string Identifier { get; }

        public Proposal PromisedProposal => _promisedProposal.Value!;
        public long PromisedProposalNumber => PromisedProposal.Number;


        public Task ReceiveAcceptRequestAsync(AcceptRequest<T> request) => throw new NotImplementedException();
        public virtual Task<PrepareResponse> ReceivePrepareRequestAsync(PrepareRequest request)
        {
            if (request.Proposal.Number > PromisedProposalNumber)
            {
                _promisedProposal.Value = request.Proposal;
                return Task.FromResult(new PrepareResponse(true, PromisedProposal));
            }

            return Task.FromResult(new PrepareResponse(false, PromisedProposal));
        }


        public override string ToString() => $"{Identifier}: {PromisedProposal}";
    }
}
