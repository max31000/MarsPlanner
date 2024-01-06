using System;
using Components;
using Components.Input;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems.Camera
{
    internal class CameraSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CameraComponent>> cameraComponentsFilter = null;
        private readonly EcsPoolInject<CameraComponent> cameraComponentsPool = null;
        private readonly EcsFilterInject<Inc<InputKeyPressedEvent>> pressedKeyEventsFilter = null;
        private readonly EcsPoolInject<InputKeyPressedEvent> pressedKeyEventsPool = null;

        public void Run(IEcsSystems systems)
        {
            foreach (var cameraId in cameraComponentsFilter.Value)
            {
                ref var cameraComponent = ref cameraComponentsPool.Value.Get(cameraId);
                UpdateCamera(ref cameraComponent);
            }
        }

        private void UpdateCamera(ref CameraComponent cameraComponent)
        {
            var leftRightDirection = 0f;
            var aheadBackDirection = 0f;
            var verticalDirection = 0f;
            var rotateDirection = 0f;

            foreach (var pressedKeysId in pressedKeyEventsFilter.Value)
            {
                ref var key = ref pressedKeyEventsPool.Value.Get(pressedKeysId).Code;

                aheadBackDirection += CalculateDirection(KeyCode.W, KeyCode.S, key);
                leftRightDirection += CalculateDirection(KeyCode.D, KeyCode.A, key);

                verticalDirection += CalculateDirection(KeyCode.Keypad2, KeyCode.Keypad8, key);

                rotateDirection += CalculateDirection(KeyCode.E, KeyCode.Q, key);
            }

            verticalDirection += -UnityEngine.Input.mouseScrollDelta.y;

            CalculateAndMoveCamera(rotateDirection, leftRightDirection, aheadBackDirection, verticalDirection,
                ref cameraComponent);
            CorrectCamera(ref cameraComponent);
        }

        private int CalculateDirection(KeyCode positiveKey, KeyCode negativeKey, KeyCode inputKey)
        {
            var direction = 0;

            if (inputKey == positiveKey)
                direction++;
            if (inputKey == negativeKey)
                direction--;

            return direction;
        }

        private void CalculateAndMoveCamera(float rotateDirection, float leftRightDirection, float aheadBackDirection,
            float verticalDirection, ref CameraComponent cameraComponent)
        {
            var movement = new Vector3(leftRightDirection, 0, aheadBackDirection) * cameraComponent.speed;
            movement = Vector3.ClampMagnitude(movement, cameraComponent.speed) * Time.deltaTime;
            var verticalDelta = verticalDirection * cameraComponent.verticalMoveStepFactor;

            if (Math.Abs(verticalDelta) != 0)
                if (Math.Abs(cameraComponent.distanceToTargetCameraHeigth + verticalDelta) <
                    CameraComponent.InertionCameraMultiplier)
                    cameraComponent.distanceToTargetCameraHeigth += verticalDelta;

            var verticalShift = cameraComponent.distanceToTargetCameraHeigth *
                                cameraComponent.cameraConvergenceWithTargetValueSpeed * Time.deltaTime;

            if (Math.Abs(cameraComponent.distanceToTargetCameraHeigth) > 0.5f)
            {
                movement.y = verticalShift;
                cameraComponent.distanceToTargetCameraHeigth -= verticalShift;
            }

            if (movement.y > 0 && cameraComponent.Camera.transform.position.y > cameraComponent.maxHeigth)
                movement.y = 0;

            if (movement.y < 0 && cameraComponent.Camera.transform.position.y < cameraComponent.minHeigth)
                movement.y = 0;

            var rotateDelta = rotateDirection * cameraComponent.rotateSpeed * Time.deltaTime;

            MoveCamera(rotateDelta, movement, cameraComponent);
        }

        private void MoveCamera(float rotateDelta, Vector3 movement, CameraComponent cameraComponent)
        {
            cameraComponent.Camera.transform.Rotate(Vector3.up * rotateDelta,
                Space.World);

            var angle = cameraComponent.Camera.transform.rotation;
            cameraComponent.Camera.transform.SetPositionAndRotation(cameraComponent.Camera.transform.position,
                Quaternion.Euler(0, angle.eulerAngles.y, angle.eulerAngles.z));
            cameraComponent.Camera.transform.Translate(movement.x, 0, movement.z, Space.Self);
            cameraComponent.Camera.transform.SetPositionAndRotation(cameraComponent.Camera.transform.position, angle);

            cameraComponent.Camera.transform.Translate(0, movement.y, 0, Space.World);
            cameraComponent.Camera.transform.Rotate(Vector3.right,
                movement.y * CameraComponent.rotateCameraMultiplier /
                (cameraComponent.maxHeigth - cameraComponent.minHeigth));
        }

        private void CorrectCamera(ref CameraComponent cameraComponent)
        {
            var cameraTransform = cameraComponent.Camera.transform;
            var camX = cameraTransform.position.x;
            var camZ = cameraTransform.position.z;

            if (camX < cameraComponent.minX)
                camX = cameraComponent.minX;
            if (camX > cameraComponent.maxX)
                camX = cameraComponent.maxX;
            if (camZ < cameraComponent.minZ)
                camZ = cameraComponent.minZ;
            if (camZ > cameraComponent.maxZ)
                camZ = cameraComponent.maxZ;

            cameraTransform.position = new Vector3(camX, cameraTransform.position.y, camZ);
        }
    }
}