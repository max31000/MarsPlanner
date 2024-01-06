using Components;
using Components.Input;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Systems.Input
{
    public class RaycastCoordinatesDetectSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CameraComponent>> cameraComponentsFilter = null;
        private readonly EcsPoolInject<CameraComponent> cameraComponentsPool = null;

        private readonly EcsFilterInject<Inc<RaycastTargetComponent, CoordinatesCollectorComponent>>
            coordinatesCollectorFilter = null;

        private readonly EcsPoolInject<CoordinatesCollectorComponent> coordinatesCollectorPool = null;
        private readonly EcsPoolInject<RaycastTargetComponent> raycastCoordinatesPool = null;


        public void Run(IEcsSystems systems)
        {
            if (coordinatesCollectorFilter.Value.GetEntitiesCount() != 1)
                return;

            ref var raycastCoordinatesComponent =
                ref raycastCoordinatesPool.Value.Get(coordinatesCollectorFilter.Value.Single());

            ref var coordinatesCollectorComponent =
                ref coordinatesCollectorPool.Value.Get(coordinatesCollectorFilter.Value.Single());

            var cameraEntity = cameraComponentsFilter.Value.Single();
            ref var cameraComponent = ref cameraComponentsPool.Value.Get(cameraEntity);
            var ray = cameraComponent.Camera.ScreenPointToRay(UnityEngine.Input.mousePosition);

            var raycastResult = raycastCoordinatesComponent.RaycastTarget.Raycast(ray, out var hit, 400);
            var isSkipUiRaycast = EventSystem.current.IsPointerOverGameObject() &&
                                  coordinatesCollectorComponent.TerrainRaycastIntersectCoordinates != default;

            if (isSkipUiRaycast || raycastResult)
                return;

            var buildingPosition = hit.point;
            coordinatesCollectorComponent.TerrainRaycastIntersectCoordinates.x = Mathf.Round(buildingPosition.x);
            coordinatesCollectorComponent.TerrainRaycastIntersectCoordinates.y = Mathf.Round(buildingPosition.y);
            coordinatesCollectorComponent.TerrainRaycastIntersectCoordinates.z = Mathf.Round(buildingPosition.z);
        }
    }
}