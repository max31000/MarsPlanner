using UnityEngine;

namespace Definitions
{
    [CreateAssetMenu(menuName = "Definitions/CameraDefinitions", fileName = "CameraDefinitions")]
    public class CameraDefinition : ScriptableObject
    {
        public float speed;
        public float verticalMoveStepFactor;
        public float rotateSpeed;
        public float maxHeigth;
        public float minHeigth;
        public float verticalMoveSpeed;
        public Camera mainCameraPrefab;
        
        public int minX; 
        public int maxX;
        public int minZ;
        public int maxZ;
    }
}