using System;

namespace Paxos.Core.Contracts
{
    public interface IProposerNumberGenerator
    {
        long Next();
        long Next(long minValue);
    }

    public class EpochProposerNumberGenerator : IProposerNumberGenerator
    {
        public long Next() => new DateTimeOffset().ToUnixTimeMilliseconds();
        public long Next(long minValue) => Next() + minValue;
    }
}