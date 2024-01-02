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
            cameraComponent.maxHeigth = cameraDefinition.maxHeigth;
            cameraComponent.minHeigth = cameraDefinition.minHeigth;
            cameraComponent.rotateSpeed = cameraDefinition.rotateSpeed;
            cameraComponent.speed = cameraDefinition.speed;
            cameraComponent.cameraConvergenceWithTargetValueSpeed = cameraDefinition.cameraConvergenceWithTargetValueSpeed;
            cameraComponent.verticalMoveStepFactor = cameraDefinition.verticalMoveStepFactor;

            cameraComponent.Camera = Object.Instantiate(cameraDefinition.mainCameraPrefab);

            cameraComponent.minX = cameraDefinition.minX;
            cameraComponent.maxX = cameraDefinition.maxX;
            cameraComponent.minZ = cameraDefinition.minZ;
            cameraComponent.maxZ = cameraDefinition.maxZ;

            // ReSharper disable once Unity.InefficientPropertyAccess
            cameraComponent.Camera.transform.position = new Vector3(position.x, cameraComponent.Camera.transform.position.y, position.z);
        }
    }
}