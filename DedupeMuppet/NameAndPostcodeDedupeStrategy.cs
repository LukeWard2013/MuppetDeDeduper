namespace DedupeMuppet
{
    public class NameAndPostcodeDedupeStrategy : IDedupeStrategy
    {
        public string Signature(Company company)
        {
            return company.Name + ":" + company.PostCode;
        }
    }
}