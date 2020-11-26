namespace Paxos.Core
{
    public record Proposal(long Number)
    {
        public T? As<T>() where T: class => this as T;
    }
    public record Proposal<TValue>(long Number, TValue Value) : Proposal(Number);

    //public record PrepareRequest<TValue>(Proposal<TValue> Proposal);
    public record PrepareRequest(Proposal Proposal);
    public record PrepareResponse(bool Promised, Proposal? AcceptedProposal);
    
    public record AcceptRequest<TValue>(Proposal<TValue> Proposal);
    public record AcceptResponse(bool Accepted);
}
