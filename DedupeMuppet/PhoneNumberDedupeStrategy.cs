namespace DedupeMuppet
{
    public class PhoneNumberDedupeStrategy : IDedupeStrategy
    {
        public string Signature(Customer customer)
        {
            return customer.Telephone;
        }
    }
}