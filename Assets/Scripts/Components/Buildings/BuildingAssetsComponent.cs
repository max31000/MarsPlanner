using System.Collections.Generic;
using Models.Buildings;
using UnityEngine;

namespace Components.Buildings
{
    public struct BuildingAssetsComponent
    {
        public Dictionary<BuildingType, GameObject> BuildingsAssets;
    }
}