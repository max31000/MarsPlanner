using UnityEngine;

namespace Models.Navigation
{
    public class NavigationNode
    {
        public Vector3 Position { get; set; }

        public NavigationNode(Vector3 position)
        {
            Position = position;
        }
    }
}