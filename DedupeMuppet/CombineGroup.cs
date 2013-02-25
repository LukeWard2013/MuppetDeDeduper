using System.Collections.Generic;
using Should.Core.Exceptions;

namespace DedupeMuppet
{
    public class CombineGroup
    {
        IEnumerable<IEnumerable<int>> _groups;

        public CombineGroup(IEnumerable<IEnumerable<int>> list)
        {
            _groups = list;
        }

        public void ShouldContainGroup(params int[] companyIds)
        {
            foreach (IEnumerable<int> list in _groups)
            {
                if (list.ArraysEqual(companyIds)) return;
            }

            throw new AssertException("fucked up");
        }
    }
}