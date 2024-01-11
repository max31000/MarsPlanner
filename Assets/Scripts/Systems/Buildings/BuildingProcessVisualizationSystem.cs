using Components.Buildings;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems.Buildings
{
    public class BuildingProcessVisualizationSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> buildPlaceFilter = null;
        private readonly EcsPoolInject<PlaceBuildProcessingComponent> buildPlacePool = null;

        private readonly EcsFilterInject<Inc<BuildingsBufferComponent>> buildingsBufferFilter = null;
        private readonly EcsPoolInject<BuildingsBufferComponent> buildingsBufferPool = null;
        
        public void Run(IEcsSystems systems)
        {
            ref var buildingBuffer = ref buildingsBufferPool.Value.Get(buildingsBufferFilter.Value.Single());

            foreach (var buildPlaceComponent in buildPlaceFilter.Value)
            {
                ref var buildingPlaceComponent = ref buildPlacePool.Value.Get(buildPlaceComponent);
                var buildingBufferObject = buildingBuffer.BuildingsBuffer[buildingPlaceComponent.Type].InstancedBuilding;

                buildingBufferObject.transform.position = buildingPlaceComponent.Position;
                buildingBufferObject.transform.rotation = Quaternion.Euler(buildingPlaceComponent.Rotation);
            }
        }
    }
}