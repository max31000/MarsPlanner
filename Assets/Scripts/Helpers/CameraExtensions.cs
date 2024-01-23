using Components;
using Definitions;
using UnityEngine;

namespace Helpers
{
    public static class CameraExtensions
    {
        public static void Initialize(this ref CameraComponent cameraComponent, CameraDefinition cameraDefinition,
            Vector3 position)
        {
            cameraComponent.MaxHeight = cameraDefinition.maxHeigth;
            cameraComponent.MinHeight = cameraDefinition.minHeigth;
            cameraComponent.RotateSpeed = cameraDefinition.rotateSpeed;
            cameraComponent.Speed = cameraDefinition.speed;
            cameraComponent.CameraConvergenceWithTargetValueSpeed =
                cameraDefinition.cameraConvergenceWithTargetValueSpeed;
            cameraComponent.VerticalMoveStepFactor = cameraDefinition.verticalMoveStepFactor;

            cameraComponent.Camera = Object.Instantiate(cameraDefinition.mainCameraPrefab);
            cameraComponent.Position = cameraComponent.Camera.transform.position;

            cameraComponent.MinX = cameraDefinition.minX;
            cameraComponent.MaxX = cameraDefinition.maxX;
            cameraComponent.MinZ = cameraDefinition.minZ;
            cameraComponent.MaxZ = cameraDefinition.maxZ;

            // ReSharper disable once Unity.InefficientPropertyAccess
            cameraComponent.Camera.transform.position =
                new Vector3(position.x, cameraComponent.Camera.transform.position.y, position.z);
        }
    }
}