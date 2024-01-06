using UnityEngine;

namespace Components.Input
{
    public struct RaycastObjectEvent
    {
        public string GameObjectName;
        public GameObject GameObject;
        public KeyCode RaySourceKeyCode;
    }
}