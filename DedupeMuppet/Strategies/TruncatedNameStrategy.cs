namespace DedupeMuppet.Strategies
{
    public class TruncatedNameAndPostcodeStrategy : IDedupeStrategy
    {
        public string Signature(Company company)
        {
            return company.TruncatedName + ":" + company.PostCode;
        }
    }
}