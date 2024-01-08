using Components.Buildings;
using Components.Input;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems.Buildings
{
    public class BuildingPlaceSelectionSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> buildPlaceFilter = null;
        private readonly EcsPoolInject<PlaceBuildProcessingComponent> buildPlacePool = null;

        private readonly EcsPoolInject<CoordinatesCollectorComponent> coordinatesCollectorPool = null;

        private readonly EcsFilterInject<Inc<RaycastTargetComponent, CoordinatesCollectorComponent>>
            raycastCoordinatesFilter = null;

        public void Run(IEcsSystems systems)
        {
            if (buildPlaceFilter.Value.GetEntitiesCount() == 0 ||
                raycastCoordinatesFilter.Value.GetEntitiesCount() == 0)
                return;

            ref var buildComponent = ref buildPlacePool.Value.Get(buildPlaceFilter.Value.Single());
            var raycastCoordinatesEntity = raycastCoordinatesFilter.Value.Single();
            ref var coordinatesComponent = ref coordinatesCollectorPool.Value.Get(raycastCoordinatesEntity);

            buildComponent.Position = coordinatesComponent.TerrainRaycastIntersectCoordinates;
        }
    }
}