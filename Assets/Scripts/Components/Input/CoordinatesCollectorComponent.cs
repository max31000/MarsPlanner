using UnityEngine;

namespace Components.Input
{
    // Raycast by coordinates in process only when this component exist
    public struct CoordinatesCollectorComponent
    {
        public Vector3 TerrainRaycastIntersectCoordinates;
    }
}