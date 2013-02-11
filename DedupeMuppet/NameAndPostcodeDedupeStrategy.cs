namespace DedupeMuppet
{
    public class NameAndPostcodeDedupeStrategy : IDedupeStrategy
    {
        public string Signature(Customer customer)
        {
            return customer.Name + ":" + customer.PostCode;
        }
    }
}