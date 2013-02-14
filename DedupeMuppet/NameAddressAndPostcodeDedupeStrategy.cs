namespace DedupeMuppet
{
    public class NameAddressAndPostcodeDedupeStrategy : IDedupeStrategy
    {
        public string Signature(Company company)
        {
            return company.Name + ":" + company.Address + ":" + company.PostCode;
        }
    }
}