using Models.Buildings;
using UnityEngine;

namespace Components.Buildings
{
    public struct PlaceBuildProcessingComponent
    {
        public BuildingTypes Type;
        public Vector3 Position;
        public bool IsCantInstall;
        public Vector3 Size;
        public Vector3 Rotation;
    }
}