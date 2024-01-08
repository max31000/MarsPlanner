using Components.Buildings;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

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
            if(buildPlaceFilter.Value.GetEntitiesCount() == 0)
                return;

            ref var buildingPlaceComponent = ref buildPlacePool.Value.Get(buildPlaceFilter.Value.Single());

            ref var buildingBuffer = ref buildingsBufferPool.Value.Get(buildingsBufferFilter.Value.Single());
            var buildingBufferObject = buildingBuffer.BuildingsBuffer[buildingPlaceComponent.Type].InstancedBuilding;

            buildingBufferObject.transform.position = buildingPlaceComponent.Position;
        }
    }
}