namespace Paxos.Core.Tests
{
    internal class SampleAcceptor : Acceptor<string>
    {
        public SampleAcceptor(string identifier)
        {
            Identifier = identifier;
        }
        public override string Identifier { get; }
    }
}
