namespace DedupeMuppet
{
    public interface IDedupeStrategy
    {
        string Signature(Company company);
    }
}