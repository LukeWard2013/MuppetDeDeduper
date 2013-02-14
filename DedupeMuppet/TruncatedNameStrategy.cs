namespace DedupeMuppet
{
    public class TruncatedNameStrategy : IDedupeStrategy
    {
        public string Signature(Company company)
        {
            return company.TruncatedName + ":" + company.PostCode;
        }
    }
}