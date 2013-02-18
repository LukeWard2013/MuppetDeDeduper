using System.Collections.Generic;

namespace DedupeMuppet.Strategies
{
    public class TruncateCompanyNameStrategy:ITruncateStrategy
    {
        public IEnumerable<string> CommonWords()
        {
            return new[]{ "BROTHERS", "LIMITED", "COMPANY", "BROS.", "BROS", "PLC.", "CO.", "LTD.", "LTD", "PLC", "AND", "THE", "CO" };
        }
    }
}
