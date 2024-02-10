using UnityEngine;

namespace Components.Input
{
    public struct RaycastObjectEvent
    {
        public string GameObjectName { get; set; }
        public GameObject GameObject { get; set; }
        public Vector3 RaycastPoint { get; set; }
        public KeyCode RaySourceKeyCode { get; set; }
    }
}