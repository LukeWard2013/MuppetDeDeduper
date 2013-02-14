namespace DedupeMuppet.Strategies
{
    public interface IDedupeStrategy
    {
        string Signature(Company company);
    }
}