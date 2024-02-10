using System;
using System.Collections.Generic;

namespace CustomMonoBehaviour
{
    [Serializable]
    public class LocalNavigationGraph
    {
        public List<UnityNavigationNode> Nodes = new List<UnityNavigationNode>();
        public List<UnityNavigationEdge> Edges = new List<UnityNavigationEdge>();
    }
}