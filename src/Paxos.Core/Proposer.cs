using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Paxos.Core.Contracts;

namespace Paxos.Core
{
    public class Proposer<T> : IProposer<T>
    {
        private readonly IAcceptorCollectionProvider<T> _acceptorCollectionProvider;
        private readonly IProposerNumberGenerator _numberGenerator;

        private long _number = long.MinValue;

        public Proposer(string identifier, IAcceptorCollectionProvider<T> acceptorCollectionProvider, IProposerNumberGenerator numberGenerator, TimeSpan? prepareRequestTimeout = null)
        {
            Identitifier = identifier;

            _acceptorCollectionProvider = acceptorCollectionProvider;
            _numberGenerator = numberGenerator;

            PrepareRequestTimeout = prepareRequestTimeout ?? TimeSpan.FromSeconds(5);

            Initialize();
        }

        private void Initialize() => _number = _numberGenerator.Next();

        protected virtual TimeSpan PrepareRequestTimeout { get; }
        protected string Identitifier { get; }

        protected long Number => _number;

        public Task SendAcceptRequestAsync(AcceptRequest<T> request) => throw new System.NotImplementedException();
        public async Task SendPrepareRequestAsync()
        {
            var acceptors = await _acceptorCollectionProvider
                .GetAcceptorsAsync()
                .ConfigureAwait(false);

            var prepareRequestTasks = acceptors.Select(acceptor => SendPrepareRequestToAcceptor(acceptor));

            var acceptorResponses = await Task.WhenAll(prepareRequestTasks).ConfigureAwait(false);

            if (PrepareResponseIsPromisedByMajorityOfAcceptors(acceptorResponses, acceptors.Length))
            {
                // happy, send accept request
            }
            else
            {
                // rejected, number is too low
                // figure out the biggest promised proposal and abandon
            }

            static bool PrepareResponseIsPromisedByMajorityOfAcceptors(PrepareResponse[] acceptorResponses, int numberOfAcceptors)
            {
                return acceptorResponses.Count(resp => resp.Promised) >= Math.Ceiling(numberOfAcceptors / 2d);
            }
        }

        public async Task<PrepareResponse> SendPrepareRequestToAcceptor(IAcceptor<T> acceptor)
        {
            try
            {
                var acceptorTask = acceptor.ReceivePrepareRequestAsync(new PrepareRequest(new Proposal(Number)));
                var prepareResponse = await acceptorTask.TimeoutAfter(PrepareRequestTimeout).ConfigureAwait(false);
                return prepareResponse;
            }
            catch (TimeoutException)
            {
                return new PrepareResponse(false, null);
            }
        }
    }
}
