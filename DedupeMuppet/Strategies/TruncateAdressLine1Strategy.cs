using System.Collections.Generic;

namespace DedupeMuppet.Strategies
{
    public class TruncateAdressLine1Strategy:ITruncateStrategy
    {
        public IEnumerable<string> CommonWords()
        {
            return new[] { " AVENUE", " STREET", " DRIVE", " DRV.", " PLC.", " LANE", " ROAD", " DRV", " Dr.", " PLC", " AND", "THE ", " AVE", " AV.", " STR", " RD.", " AV", " Dr", " ST", " LN.", " RD" };
        }
    }
}
