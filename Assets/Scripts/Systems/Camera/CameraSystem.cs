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
        private readonly EcsFilterInject<Inc<WorldTerrainKeeperComponent>> terrainKeeperFilter = null;

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
            var movement = new Vector3(leftRightDirection, 0, aheadBackDirection) * cameraComponent.Speed;
            movement = Vector3.ClampMagnitude(movement, cameraComponent.Speed) * Time.deltaTime;
            var verticalDelta = verticalDirection * cameraComponent.VerticalMoveStepFactor;

            if (Math.Abs(verticalDelta) != 0)
                if (Math.Abs(cameraComponent.DistanceToTargetCameraHeight + verticalDelta) <
                    CameraComponent.InertionCameraMultiplier)
                    cameraComponent.DistanceToTargetCameraHeight += verticalDelta;

            var verticalShift = cameraComponent.DistanceToTargetCameraHeight *
                                cameraComponent.CameraConvergenceWithTargetValueSpeed * Time.deltaTime;

            if (Math.Abs(cameraComponent.DistanceToTargetCameraHeight) > 0.5f)
            {
                movement.y = verticalShift;
                cameraComponent.DistanceToTargetCameraHeight -= verticalShift;
            }

            if (movement.y > 0 && cameraComponent.Position.y > cameraComponent.MaxHeight)
                movement.y = 0;

            if (movement.y < 0 && cameraComponent.Position.y < cameraComponent.MinHeight)
                movement.y = 0;

            var rotateDelta = rotateDirection * cameraComponent.RotateSpeed * Time.deltaTime;

            MoveCamera(rotateDelta, movement, ref cameraComponent);
        }

        private void MoveCamera(float rotateDelta, Vector3 movement, ref CameraComponent cameraComponent)
        {
            var cameraTransform = cameraComponent.Camera.transform;

            var movementVector = new Vector2(movement.x, movement.z);

            var cameraAngle = cameraTransform.eulerAngles.y;
            var radians = cameraAngle * Mathf.Deg2Rad;

            var cos = Mathf.Cos(-radians);
            var sin = Mathf.Sin(-radians);
            var rotatedVector = new Vector2(
                movementVector.x * cos - movementVector.y * sin,
                movementVector.x * sin + movementVector.y * cos
            );

            cameraComponent.Position += new Vector3(rotatedVector.x, movement.y, rotatedVector.y);

            cameraTransform.Rotate(Vector3.up * rotateDelta,
                Space.World);

            var angle = cameraTransform.rotation;
            cameraTransform.SetPositionAndRotation(cameraTransform.position,
                Quaternion.Euler(0, angle.eulerAngles.y, angle.eulerAngles.z));
            cameraTransform.SetPositionAndRotation(cameraTransform.position, angle);

            cameraTransform.Rotate(Vector3.right,
                movement.y * CameraComponent.RotateCameraMultiplier /
                (cameraComponent.MaxHeight - cameraComponent.MinHeight));
        }
    }
}