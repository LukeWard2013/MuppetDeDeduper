namespace DedupeMuppet.Strategies
{
    public class NameAddressAndPostcodeDedupeStrategy : IDedupeStrategy
    {
        public string Signature(Company company)
        {
            return company.Name + ":" + company.Address + ":" + company.PostCode;
        }
    }
}