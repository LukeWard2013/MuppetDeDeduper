using System.Collections.Generic;
using System.Linq;

namespace DedupeMuppet
{
    public class Deduper
    {
        private readonly IDedupeStrategy[] _dedupeStrategies;

        public Deduper(params IDedupeStrategy[] dedupeStrategies)
        {
            _dedupeStrategies = dedupeStrategies;
        }

        public IEnumerable<GroupedCustomer> Dedupe(IEnumerable<Customer> customers)
        {
            return from customer in customers
                   from strategy in _dedupeStrategies
                   group customer by new StrategySignature(strategy, strategy.Signature(customer))
                   
                   into grouped
                   where grouped.Count() >= 2
                   select new GroupedCustomer
                       {
                           Signature = grouped.Key.Signature,
                           Dupe = grouped,
                           Qty = grouped.Count(),
                           StrategyType = grouped.Key.Strategy.GetType()
                       };
        }
    }
}