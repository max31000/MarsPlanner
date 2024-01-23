using Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems.Camera
{
    public class UnderCameraHeightCalculateSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CameraComponent>> cameraComponentsFilter = null;

        public void Init(IEcsSystems systems)
        {
            foreach (var cameraEntity in cameraComponentsFilter.Value)
            {
                ref var cameraComponent = ref cameraComponentsFilter.Pools.Inc1.Get(cameraEntity);

                SetUnderCameraHeight(ref cameraComponent);
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var cameraEntity in cameraComponentsFilter.Value)
            {
                ref var cameraComponent = ref cameraComponentsFilter.Pools.Inc1.Get(cameraEntity);

                if ((cameraComponent.Camera.transform.position - cameraComponent.Position).magnitude < 0.001f) return;

                SetUnderCameraHeight(ref cameraComponent);
            }
        }

        private static void SetUnderCameraHeight(ref CameraComponent cameraComponent)
        {
            var downRay = new Ray(cameraComponent.Camera.transform.position, Vector3.down);
            var height = 0f;

            var layerMask = LayerMask.GetMask("Terrain");
            if (Physics.Raycast(downRay, out var hit, 400, layerMask))
                height = hit.point.y;

            cameraComponent.UnderCameraTerrainHeight = height;
        }
    }
}