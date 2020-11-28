namespace Paxos.Core.Contracts
{
    public interface IProposerNumberGenerator
    {
        long Next();
        long Next(long minValue);
    }
}