namespace Paxos.Core
{
    public record Proposal(long Number);
    public record Proposal<TValue>(long Number, TValue Value) : Proposal(Number);
    public record PrepareRequest(long Number);
    public record PrepareResponse(bool Promised, Proposal? AcceptedProposal);
    public record AcceptRequest<TValue>(Proposal<TValue> Proposal);
    public record AcceptResponse(bool Accepted);
}
