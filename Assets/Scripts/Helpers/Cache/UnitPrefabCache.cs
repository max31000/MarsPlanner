using System.Collections.Generic;
using Models.Units;
using UnityEngine;

namespace Helpers.Cache
{
    public static class UnitPrefabCache
    {
        private static Dictionary<UnitType, GameObject> unitPrefabByType;

        public static GameObject GetPrefab(UnitType type)
        {
            unitPrefabByType ??= new Dictionary<UnitType, GameObject>();

            if (!unitPrefabByType.ContainsKey(type))
                unitPrefabByType.Add(type, Resources.Load<GameObject>($"Prefabs/Units/Unit{type.ToString()}"));

            return unitPrefabByType[type];
        }
    }
}