using UnityEngine;

namespace Components
{
    public struct CameraComponent
    {
        public Camera Camera;

        public static readonly float InertionCameraMultiplier = 8f;

        public static readonly int rotateCameraMultiplier = 45;

        public float distanceToTargetCameraHeigth;
        public float maxHeigth;
        public float minHeigth;
        public float rotateSpeed;
        public float speed;
        public float cameraConvergenceWithTargetValueSpeed;
        public float verticalMoveStepFactor;
        
        public int minX; 
        public int maxX;
        public int minZ;
        public int maxZ;
    }
}