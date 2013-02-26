using System;
using System.Collections.Generic;
using System.Linq;

namespace DedupeMuppet
{
    public class CombineGroup
    {
        IEnumerable<IEnumerable<Company>> _groups;

        public CombineGroup(IEnumerable<IEnumerable<Company>> list)
        {
            _groups = list;
        }

        public void ShouldContainGroup(params int[] companyIds)
        {
            foreach (IEnumerable<Company> list in _groups)
            {
                if (list.Select(c => c.Id).ArraysEqual(companyIds)) return;
            }

            foreach (IEnumerable<Company> list in _groups)
            {
                Console.WriteLine("Found " + string.Join(",", list.Select(c => c.Id)));    
            }
            
            throw new NoGroupException("Expected " + string.Join(",", companyIds));
        }
    }

    public class NoGroupException : Exception
    {
        public NoGroupException(string message) : base(message)
        {
            
        }
    }
}