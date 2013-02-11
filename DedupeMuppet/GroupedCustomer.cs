using System;
using System.Linq;

namespace DedupeMuppet
{
    public class GroupedCustomer
    {
        public string Signature { get; set; }
        public IGrouping<StrategySignature, Customer> Dupe { get; set; }
        public int Qty { get; set; }
        public Type StrategyType { get; set; }

    }
}