using UnityEngine;

namespace Definitions
{
    [CreateAssetMenu(menuName = "Definitions/GameDefinitions", fileName = "CameraDefinitions")]
    public class GameDefinitions : ScriptableObject
    {
        public CameraDefinition CameraDefinitions;

        public KeysDefinitions KeysDefinitions;
    }
}