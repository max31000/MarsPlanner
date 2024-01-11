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
            if (raycastCoordinatesFilter.Value.GetEntitiesCount() == 0)
                return;

            foreach (var buildPlaceComponent in buildPlaceFilter.Value)
            {
                ref var buildComponent = ref buildPlacePool.Value.Get(buildPlaceComponent);
                var raycastCoordinatesEntity = raycastCoordinatesFilter.Value.Single();
                ref var coordinatesComponent = ref coordinatesCollectorPool.Value.Get(raycastCoordinatesEntity);

                buildComponent.Position = new Vector3(
                    Mathf.Round(coordinatesComponent.TerrainRaycastIntersectCoordinates.x),
                    coordinatesComponent.TerrainRaycastIntersectCoordinates.y,
                    Mathf.Round(coordinatesComponent.TerrainRaycastIntersectCoordinates.z)
                );
            }
        }
    }
}