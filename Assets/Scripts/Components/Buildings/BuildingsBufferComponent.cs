using System.Collections.Generic;
using Models.Buildings;

namespace Components.Buildings
{
    public struct BuildingsBufferComponent
    {
        public Dictionary<BuildingTypes, BuildingBuffer> BuildingsBuffer;
    }
}