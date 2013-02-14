using System;
using DedupeMuppet.Strategies;

namespace DedupeMuppet
{
    public class StrategySignature
    {
        protected bool Equals(StrategySignature other)
        {
            return string.Equals(Signature, other.Signature) && Equals(Strategy, other.Strategy);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Signature != null ? Signature.GetHashCode() : 0)*397) ^ (Strategy != null ? Strategy.GetHashCode() : 0);
            }
        }

        public IDedupeStrategy Strategy { get; set; }
        public string Signature { get; set; }

        public StrategySignature(IDedupeStrategy strategy, string signature)
        {
            Strategy = strategy;
            Signature = signature;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StrategySignature) obj);
        }
    }
}