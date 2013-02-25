using System.Collections.Generic;
using System.Linq;

namespace DedupeMuppet
{
    public static class EnumerableExtensions
    {
        public static bool ArraysEqual<T>(this IEnumerable<T> arg1, IEnumerable<T> arg2)
        {
            if (arg1 == null || arg2 == null)
                return false;

            var a1 = arg1.ToArray();
            var a2 = arg2.ToArray();

            if (ReferenceEquals(a1, a2))
                return true;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }
    }
}
