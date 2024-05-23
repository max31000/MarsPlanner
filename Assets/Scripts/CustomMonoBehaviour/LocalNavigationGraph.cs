using System;
using System.Collections.Generic;

namespace CustomMonoBehaviour
{
    [Serializable]
    public class LocalNavigationGraph
    {
        public List<UnityNavigationNode> Nodes = new();
        public List<UnityNavigationEdge> Edges = new();
    }
}