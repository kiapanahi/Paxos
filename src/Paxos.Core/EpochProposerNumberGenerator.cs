using System;
using Paxos.Core.Contracts;

namespace Paxos.Core
{
    public class EpochProposerNumberGenerator : IProposerNumberGenerator
    {
        public long Next() => new DateTimeOffset().ToUnixTimeMilliseconds();
        public long Next(long minValue) => Next() + minValue;
    }
}