using Components.Buildings;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems.Buildings
{
    public class BuildBufferBoundsUpdateSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> buildPlaceFilter = null;
        private readonly EcsFilterInject<Inc<BuildingsBufferComponent>> buildingBufferFilter = null;
        
        public void Run(IEcsSystems systems)
        {
            if (buildPlaceFilter.Value.GetEntitiesCount() == 0)
                return;

            ref var buildBuffer = ref buildingBufferFilter.Pools.Inc1.Get(buildingBufferFilter.Value.Single());
            ref var buildComponent = ref buildPlaceFilter.Pools.Inc1.Get(buildPlaceFilter.Value.Single());
            var buildBufferObject = buildBuffer.BuildingsBuffer[buildComponent.Type];

            var bufferSize = buildBufferObject.InstancedBuilding.GetComponentInChildren<Collider>().bounds;
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (buildComponent.Bounds != bufferSize)
            {
                buildComponent.Bounds = bufferSize;
            }
        }
    }
}