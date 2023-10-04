using System;
using System.Collections.Generic;
using System.Linq;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        public static IEnumerable<T> TakeOrDefault<T>(this IEnumerable<T> enumerable, int length)
        {
            int count = 0;
            foreach (T element in enumerable)
            {
                if (count == length)
                    yield break;

                yield return element;
                count++;
            }

            while (count != length)
            {
                yield return default;
                count++;
            }
        }

        private static readonly Random RandomGenerator = new Random(unchecked((int)DateTime.Now.Ticks));

        public static T RandomOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            IEnumerable<T> filtered = source.Where(predicate);

            int count = 0;
            T selected = default(T);
            foreach (T current in filtered)
            {
                if (RandomGenerator.Next(0, ++count) == 0)
                {
                    selected = current;
                }
            }

            return selected;
        }

        public static T RandomOrDefault<T>(this IEnumerable<T> source)
        {
            return RandomOrDefault(source, element => true);
        }
    }
}
