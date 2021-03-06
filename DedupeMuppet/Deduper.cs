﻿using System.Collections.Generic;
using System.Linq;
using DedupeMuppet.Strategies;

namespace DedupeMuppet
{
    public class Deduper
    {
        private readonly IDedupeStrategy[] _dedupeStrategies;

        public Deduper(params IDedupeStrategy[] dedupeStrategies)
        {
            _dedupeStrategies = dedupeStrategies;
        }

        public IEnumerable<IGrouping<StrategySignature, Company>> Dedupe(IEnumerable<Company> customers)
        {
            return from customer in customers
                   from strategy in _dedupeStrategies
                   group customer by new StrategySignature(strategy, strategy.Signature(customer))
                   into grouped
                   where grouped.Count() >= 2
                   select grouped;
        }
    }
}