using System;
using Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    internal class CameraSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CameraComponent>> cameraComponentsFilter = null;
        private readonly EcsPoolInject<CameraComponent> cameraComponentsPool = null;
        private readonly EcsFilterInject<Inc<PressKeyEvent>> pressedKeyEventsFilter = null;
        private readonly EcsPoolInject<PressKeyEvent> pressedKeyEventsPool = null;

        public void Run(IEcsSystems systems)
        {
            foreach (var cameraId in cameraComponentsFilter.Value)
            {
                ref var cameraComponent = ref cameraComponentsPool.Value.Get(cameraId);
                UpdateCamera(cameraComponent);
            }
        }

        private void UpdateCamera(CameraComponent cameraComponent)
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

            verticalDirection += -Input.mouseScrollDelta.y;

            CalculateAndMoveCamera(rotateDirection, leftRightDirection, aheadBackDirection, verticalDirection,
                cameraComponent);
            CorrectCamera(cameraComponent);
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
            float verticalDirection, CameraComponent cameraComponent)
        {
            var movement = new Vector3(leftRightDirection, 0, aheadBackDirection) * cameraComponent.speed;
            movement = Vector3.ClampMagnitude(movement, cameraComponent.speed) * Time.deltaTime;
            var verticalDelta = verticalDirection * cameraComponent.verticalMoveStepFactor * Time.deltaTime;

            if (Math.Abs(verticalDelta) > cameraComponent.verticalMoveSpeed)
            {
                movement.y = Math.Sign(verticalDelta) * cameraComponent.verticalMoveSpeed;
                if (Math.Abs(cameraComponent.distanceToTargetCameraHeigth) <
                    cameraComponent.verticalMoveSpeed * CameraComponent.InertionCameraMultiplier)
                    cameraComponent.distanceToTargetCameraHeigth += verticalDelta - cameraComponent.verticalMoveSpeed;
            }
            else
            {
                movement.y = verticalDelta;
            }

            if (Math.Abs(cameraComponent.distanceToTargetCameraHeigth) > cameraComponent.verticalMoveSpeed * 2)
            {
                movement.y = Math.Sign(cameraComponent.distanceToTargetCameraHeigth) *
                             cameraComponent.verticalMoveSpeed;
                cameraComponent.distanceToTargetCameraHeigth -=
                    Math.Sign(cameraComponent.distanceToTargetCameraHeigth) * cameraComponent.verticalMoveSpeed;
            }

            if (movement.y > 0 && cameraComponent.Camera.transform.position.y > cameraComponent.maxHeigth)
                movement.y = 0;

            if (movement.y < 0 && cameraComponent.Camera.transform.position.y < cameraComponent.minHeigth)
                movement.y = 0;

            MoveCamera(rotateDirection, movement, cameraComponent);
        }

        private void MoveCamera(float rotateDirection, Vector3 movement, CameraComponent cameraComponent)
        {
            cameraComponent.Camera.transform.Rotate(Vector3.up * rotateDirection * cameraComponent.rotateSpeed,
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

        private void CorrectCamera(CameraComponent cameraComponent)
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