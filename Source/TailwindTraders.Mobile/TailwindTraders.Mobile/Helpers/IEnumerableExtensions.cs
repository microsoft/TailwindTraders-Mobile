using System;
using System.Collections.Generic;
using System.Linq;

namespace TailwindTraders.Mobile.Helpers
{
    internal static class IEnumerableExtensions
    {
        private static readonly Random Random = new Random();

        // Taken from: https://stackoverflow.com/a/5807238/5831716
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            var buffer = enumerable.ToList();

            for (var i = 0; i < buffer.Count; i++)
            {
                var j = Random.Next(i, buffer.Count);

                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }
    }
}
