namespace DedupeMuppet
{
    public class PhoneNumberDedupeStrategy : IDedupeStrategy
    {
        public string Signature(Company company)
        {
            return company.Telephone;
        }
    }
}