using Components;
using Components.World;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems.Camera
{
    public class CameraPositionApplierSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CameraComponent>> cameraComponentsFilter = null;

        public void Run(IEcsSystems systems)
        {
            foreach (var cameraEntity in cameraComponentsFilter.Value)
            {
                ref var cameraComponent = ref cameraComponentsFilter.Pools.Inc1.Get(cameraEntity);
                var cameraTransform = cameraComponent.Camera.transform;

                cameraTransform.SetPositionAndRotation(
                    new Vector3(cameraComponent.Position.x,
                        cameraComponent.Position.y + cameraComponent.UnderCameraTerrainHeight,
                        cameraComponent.Position.z),
                    cameraTransform.rotation);

                CorrectCamera(ref cameraComponent);
            }
        }

        private void CorrectCamera(ref CameraComponent cameraComponent)
        {
            var cameraTransform = cameraComponent.Camera.transform;
            var camX = cameraTransform.position.x;
            var camZ = cameraTransform.position.z;

            if (camX < cameraComponent.MinX)
                camX = cameraComponent.MinX;
            if (camX > cameraComponent.MaxX)
                camX = cameraComponent.MaxX;
            if (camZ < cameraComponent.MinZ)
                camZ = cameraComponent.MinZ;
            if (camZ > cameraComponent.MaxZ)
                camZ = cameraComponent.MaxZ;

            cameraTransform.position = new Vector3(camX, cameraTransform.position.y, camZ);
        }
    }
}