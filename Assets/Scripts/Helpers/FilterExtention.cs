using System;
using Leopotam.EcsLite;

namespace Helpers
{
    public static class FilterExtention
    {
        public static int Single(this EcsFilter filter)
        {
            if (filter.GetEntitiesCount() != 1)
                throw new ArgumentException("The number of elements in the collection is not equal to one");

            using var enumerator = filter.GetEnumerator();
            enumerator.MoveNext();
            return enumerator.Current;
        }

        public static bool Contains(this EcsFilter filter, int entity)
        {
            foreach (var filterEntity in filter)
                if (filterEntity == entity)
                    return true;

            return false;
        }
    }
}