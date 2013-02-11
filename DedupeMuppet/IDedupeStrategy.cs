namespace DedupeMuppet
{
    public interface IDedupeStrategy
    {
        string Signature(Customer customer);
    }
}