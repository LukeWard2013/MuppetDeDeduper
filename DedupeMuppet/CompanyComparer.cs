using System.Collections.Generic;

namespace DedupeMuppet
{
    public class CompanyComparer : IEqualityComparer<Company>
    {
        public bool Equals(Company x, Company y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(Company company)
        {
            return company.Id;
        }
    }
}