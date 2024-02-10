using System;
using Models.Buildings;
using UnityEngine;

namespace Components.Buildings
{
    public struct BuildComponent
    {
        public BuildingType Type;

        public Guid BuildId { get; set; }

        public GameObject Object;

        public BuildingGateInfo[] BuildingGateInfos;
    }
}