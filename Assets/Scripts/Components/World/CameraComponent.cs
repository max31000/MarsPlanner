using UnityEngine;

namespace Components
{
    public struct CameraComponent
    {
        public Camera Camera;
        public float UnderCameraTerrainHeight;

        public Vector3 Position;

        public const float InertionCameraMultiplier = 8f;
        public const int RotateCameraMultiplier = 45;

        public float DistanceToTargetCameraHeight;
        public float MaxHeight;
        public float MinHeight;
        public float RotateSpeed;
        public float Speed;
        public float CameraConvergenceWithTargetValueSpeed;
        public float VerticalMoveStepFactor;

        public int MinX;
        public int MaxX;
        public int MinZ;
        public int MaxZ;
    }
}