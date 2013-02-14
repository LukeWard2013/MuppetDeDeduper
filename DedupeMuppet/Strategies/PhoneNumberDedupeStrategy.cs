namespace DedupeMuppet.Strategies
{
    public class PhoneNumberDedupeStrategy : IDedupeStrategy
    {
        public string Signature(Company company)
        {
            return company.Telephone;
        }
    }
}