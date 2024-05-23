using UnityEngine;

namespace Models.Navigation
{
    public class NavigationNode
    {
        public NavigationNode(Vector3 position)
        {
            Position = position;
        }

        public Vector3 Position { get; set; }

        public NodeType Type { get; set; } = NodeType.GridNode;

        public NodeStatus Status { get; set; } = NodeStatus.Enabled;

        public float Distance(NavigationNode node)
        {
            return Vector3.Distance(Position, node.Position);
        }
    }
}