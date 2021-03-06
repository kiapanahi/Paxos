﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Paxos.Core.Tests.Acceptor
{

    public class AcceptorTests
    {
        [Fact]
        public void Acceptor_ToString()
        {
            var sut = new Acceptor<string>("sample-acceptor");

            Assert.Equal($"sample-acceptor: Proposal {{ Number = {long.MinValue} }}", sut.ToString());
        }

        [Fact]
        public void NewAcceptor_AcceptedProposal_IsNotNull()
        {
            var sut = new Acceptor<string>("sample-acceptor");
            Assert.NotNull(sut.PromisedProposal);
            Assert.Equal(long.MinValue, sut.PromisedProposalNumber);
        }

        [Fact]
        public async Task NewAcceptor_ReceivePrepareRequest_Promised()
        {
            var acceptor = new Acceptor<string>("sample-acceptor");

            var proposal = new Proposal<string>(1, "192.168.0.1");
            var request = new PrepareRequest(proposal);
            var response = await acceptor.ReceivePrepareRequestAsync(request)
                                    .ConfigureAwait(false);

            Assert.NotNull(response);
            Assert.True(response.Promised);
            Assert.NotNull(response.AcceptedProposal);
            Assert.IsType<Proposal<string>>(response.AcceptedProposal);
            Assert.IsNotType<Proposal<object>>(response.AcceptedProposal);
            Assert.IsNotType<Proposal<int>>(response.AcceptedProposal);
            Assert.Equal(proposal, response.AcceptedProposal);


            Assert.Equal(1, acceptor.PromisedProposalNumber);
            Assert.NotNull(acceptor.PromisedProposal.As<Proposal<string>>());
            Assert.Equal("192.168.0.1", acceptor.PromisedProposal.As<Proposal<string>>()!.Value);

            Assert.Equal($"sample-acceptor: Proposal {{ Number = 1, Value = 192.168.0.1 }}", acceptor.ToString());
        }

        [Fact]
        public async Task Acceptor_OlderPrepareRequest_Rejected()
        {
            var acceptor = new Acceptor<string>("sample-acceptor");

            var winningProposal = new Proposal<string>(100, "192.168.0.100");
            var winningRequest = new PrepareRequest(winningProposal);
            var winningResponse = await acceptor.ReceivePrepareRequestAsync(winningRequest)
                                    .ConfigureAwait(false);

            var newProposal = new Proposal<string>(99, "192.168.0.99");
            var newRequest = new PrepareRequest(newProposal);
            var newResponse = await acceptor.ReceivePrepareRequestAsync(newRequest)
                                    .ConfigureAwait(false);



            Assert.NotNull(newResponse);
            Assert.False(newResponse.Promised);
            Assert.NotNull(newResponse.AcceptedProposal);
            Assert.IsType<Proposal<string>>(newResponse.AcceptedProposal);
            Assert.IsNotType<Proposal<object>>(newResponse.AcceptedProposal);
            Assert.IsNotType<Proposal<int>>(newResponse.AcceptedProposal);
            Assert.Equal(winningProposal, winningResponse.AcceptedProposal);


            Assert.Equal(100, acceptor.PromisedProposalNumber);
            Assert.NotNull(acceptor.PromisedProposal.As<Proposal<string>>());
            Assert.Equal("192.168.0.100", acceptor.PromisedProposal.As<Proposal<string>>()!.Value);

            Assert.Equal($"sample-acceptor: Proposal {{ Number = 100, Value = 192.168.0.100 }}", acceptor.ToString());
        }

        [Fact]
        public async Task Acceptor_CuncurrentPrepareRequests_PromiseHighestNumber()
        {
            var rnd = new Random(42);
            var acceptor = new Acceptor<string>("sample-acceptor");

            var randomNumbers = Enumerable.Range(1, 10)
                .Select(i => rnd.Next(200, 500))
                .ToArray();

            var maxNumber = randomNumbers.Max();

            var requestTasks = randomNumbers
                .Select(i => new PrepareRequest(new Proposal<string>(i, $"proposal-{i:D5}")))
                .Select(req => acceptor.ReceivePrepareRequestAsync(req));

            _ = await Task.WhenAll(requestTasks).ConfigureAwait(false);


            Assert.Equal(maxNumber, acceptor.PromisedProposalNumber);

        }
    }
}
