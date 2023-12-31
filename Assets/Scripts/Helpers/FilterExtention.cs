using System;
using Leopotam.EcsLite;

namespace Helpers
{
    public static class FilterExtention
    {
        public static int Single(this EcsFilter filter)
        {
            if (filter.GetEntitiesCount() != 1)
            {
                throw new ArgumentException("The number of elements in the collection is not equal to one");
            }

            var enumerator = filter.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}