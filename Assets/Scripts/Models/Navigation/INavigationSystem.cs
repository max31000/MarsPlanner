using System.Collections.Generic;
using QuikGraph;
using UnityEngine;

namespace Models.Navigation
{
    public interface INavigationSystem
    {
        void DrawGraph();

        List<NavigationNode> FindPathTo(Vector3 startPoint, Vector3 destination);

        IEnumerable<NavigationNode> GetNodesInArea(Vector3 point, int width, int height, float rotationDegrees);

        NavigationNode FindClosestNode(Vector3 point, NodeType? nodeType = null, NodeStatus? nodeStatus = null);

        void AddSubgraph(UndirectedGraph<NavigationNode, Edge<NavigationNode>> subgraph,
            NavigationNode subGraphConnectingNode, NavigationNode otherConnectingNode);
    }
}