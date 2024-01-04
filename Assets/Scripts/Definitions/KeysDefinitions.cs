using UnityEngine;

namespace Definitions
{
    [CreateAssetMenu(menuName = "Definitions/KeysDefinitions", fileName = "KeysDefinitions")]
    public class KeysDefinitions : ScriptableObject
    {
        public KeyCode[] KeysWithRaycastObserving;
    }
}