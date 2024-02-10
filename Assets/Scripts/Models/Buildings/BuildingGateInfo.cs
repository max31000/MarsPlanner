using System;
using UnityEngine;

namespace Models.Buildings
{
    public class BuildingGateInfo
    {
        public Vector3 Scale { get; set; }
        
        public Vector3 Position { get; set; }
        
        public Vector3 OutDirectionPosition { get; set; }

        public Guid? AttachedBuildId { get; set; }
    }
}