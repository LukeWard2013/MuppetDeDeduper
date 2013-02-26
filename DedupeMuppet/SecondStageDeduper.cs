using System.Collections.Generic;
using System.Linq;

namespace DedupeMuppet
{
    public class SecondStageSetDeduper
    {
        private readonly IGrouping<StrategySignature, Company>[] _deduped;

        private readonly List<IEnumerable<Company>> _secondStage = new List<IEnumerable<Company>>();
 
        public SecondStageSetDeduper(IGrouping<StrategySignature, Company>[] deduped)
        {
            _deduped = deduped;
        }

        public CombineGroup Combine()
        {
            // group 1 + 2
            // do we have a group containing 1 or 2
            // if yes, add these numbers to existing group and distinct
            // if no, create a new group with these ids
            // group 1 + 3
            // do we have a group containing 1 or 3
            // if yes, add these numbers to existing group and distinct
            // if no, create a new group with these ids
            // result = group 1 + 2 + 3
            //(1 + 2).Intersects(1 + 3) = (1)
            //    (1 + 2).Union(1 + 3) = (1,2,3)
            foreach (var group in _deduped) // IEnumerable<Company>
            {
                IEnumerable<Company> match = _secondStage.FirstOrDefault(sg => group.Intersect(sg).Any());
                if (match != null)
                {
                    _secondStage[_secondStage.IndexOf(match)] = match.Union(group, new CompanyComparer());
                }
                else
                {
                    _secondStage.Add(group);    
                }
            }
            return new CombineGroup(_secondStage);
        }
    }
}