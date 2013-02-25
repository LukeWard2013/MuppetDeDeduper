using System.Collections.Generic;
using System.Linq;

namespace DedupeMuppet
{
    public class SecondStageDeduper
    {
        private readonly IGrouping<StrategySignature, Company>[] _deduped;


        private List<HashSet<int>> groups = new List<HashSet<int>>();
        private Dictionary<int, int> groupIdContainingCompany = new Dictionary<int, int>();

        public SecondStageDeduper(IGrouping<StrategySignature, Company>[] deduped)
        {
            _deduped = deduped;
        }

        public CombineGroup Combine()
        {
            int groupId = 0;
            foreach (var group in _deduped)
            {
                var foundCompanyId = 0;

                foreach (var company in group)
                {
                    if (groupIdContainingCompany.ContainsKey(company.Id))
                    {
                        foundCompanyId = company.Id;
                        break;
                    }
                }

                if (foundCompanyId == 0)
                {

                    var newGroup = new HashSet<int>(group.Select(company => company.Id));
                    groups.Add(newGroup);
                    foreach (var company in group)
                    {
                        groupIdContainingCompany.Add(company.Id, groupId);
                    }
                    groupId++;
                }
                else
                {
                    var foundGroupId = groupIdContainingCompany[foundCompanyId];
                    HashSet<int> foundGroup = groups[foundGroupId];
                    foreach (var company in group)
                    {
                        if (!foundGroup.Contains(company.Id))
                        {
                            foundGroup.Add(company.Id);
                        }
                        //Adding dictionary check here, as companies can be found multiple times
                        if (!groupIdContainingCompany.ContainsKey(company.Id))
                            groupIdContainingCompany.Add(company.Id, groupId);
                    }
                }

            }

            // group 1 + 2
            // do we have a group containing 1 or 2
            // if yes, add these numbers to existing group and distinct
            // if no, create a new group with these ids
            // group 1 + 3
            // do we have a group containing 1 or 3
            // if yes, add these numbers to existing group and distinct
            // if no, create a new group with these ids
            // result = group 1 + 2 + 3

            return new CombineGroup(groups);
        }
    }
}