using Models.Buildings;
using UnityEngine;

namespace Components.Buildings
{
    public struct PlaceBuildProcessingComponent
    {
        public BuildingType Type;
        public Vector3 Position;
        public bool IsCanInstall;
        public Bounds Bounds;
        public Vector3 Rotation;
    }
}