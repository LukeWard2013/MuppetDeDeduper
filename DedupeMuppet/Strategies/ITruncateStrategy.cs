using System.Collections.Generic;

namespace DedupeMuppet.Strategies
{
    public interface ITruncateStrategy
    {
        IEnumerable<string> CommonWords ();
    }
}
