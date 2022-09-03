using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orkan
{
    public static class ArrayUtils
    {
        public static T ArgMin<T>(this IEnumerable<T> enumerable, Func<T, int> selector)
        {
            if (enumerable.Count() == 0)
                throw new ArgumentException("Cannot work out the arg min of a enumerable of length 0");
            T min = enumerable.ElementAt(0);
            foreach (T e in enumerable)
                if (selector(e) < selector(min))
                    min = e;
            return min;
        }

        public static T ArgMax<T>(this IEnumerable<T> enumerable, Func<T, int> selector) => ArgMin(enumerable, t => -selector(t));


    }
}
