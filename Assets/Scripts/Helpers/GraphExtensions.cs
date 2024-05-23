using System.Collections.Generic;
using System.Linq;
using CustomMonoBehaviour;
using Models.Navigation;
using QuikGraph;
using UnityEngine;

namespace Helpers
{
    public static class GraphExtensions
    {
        public static void RemoveDisconnectedSubgraphs<TVertex, TEdge>(this UndirectedGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            // Идентификация всех связных подграфов
            var connectedComponents = new Dictionary<int, List<TVertex>>();
            var componentNumber = 0;

            var vertexVisited = graph.Vertices.ToDictionary(v => v, v => false);
            foreach (var vertex in graph.Vertices)
                if (!vertexVisited[vertex])
                {
                    connectedComponents[componentNumber] = new List<TVertex>();
                    Dfs(graph, vertex, vertexVisited, connectedComponents[componentNumber]);
                    componentNumber++;
                }

            // Нахождение максимального связного подграфа
            var largestComponent = connectedComponents.OrderByDescending(c => c.Value.Count).FirstOrDefault();

            // Удаление всех вершин и рёбер, не принадлежащих к максимальному связному подграфу
            foreach (var component in connectedComponents)
                if (component.Key != largestComponent.Key)
                    foreach (var vertex in component.Value)
                    {
                        var edgesToRemove = graph.AdjacentEdges(vertex).ToList();
                        foreach (var edge in edgesToRemove)
                            graph.RemoveEdge(edge);
                        graph.RemoveVertex(vertex);
                    }
        }
        
        public static UndirectedGraph<NavigationNode, Edge<NavigationNode>> ToUndirectedGraph(
            this LocalNavigationGraph localNavigationGraph, Vector3 transformPosition, float eulerAnglesY)
        {
            var graph = new UndirectedGraph<NavigationNode, Edge<NavigationNode>>();
            var nodeByIndex = new Dictionary<int, NavigationNode>();
            for (var i = 0; i < localNavigationGraph.Nodes.Count; i++)
            {
                var node = localNavigationGraph.Nodes[i];
                var position = node.position + transformPosition;
                var rotation = Quaternion.Euler(0, eulerAnglesY, 0);
                var navigationNode = node.ToNavigationNode();
                navigationNode.Position = rotation * (position - transformPosition) + transformPosition;
                nodeByIndex[i] = navigationNode;
                graph.AddVertex(navigationNode);
            }

            foreach (var edge in localNavigationGraph.Edges)
            {
                var startNode = nodeByIndex[edge.startIndex];
                var endNode = nodeByIndex[edge.endIndex];
                graph.AddEdge(new Edge<NavigationNode>(startNode, endNode));
            }

            return graph;
        }
        
        public static NavigationNode ToNavigationNode(this UnityNavigationNode node)
        {
            return new NavigationNode(node.position)
            {
                Type = node.GetNodeType()
            };
        }

        public static NodeType GetNodeType(this UnityNavigationNode node)
        {
            if (node.isBuildConnectionNode)
                return NodeType.BuildConnectionNode;
            if (node.isOutNode)
                return NodeType.BuildOutNode;
            return NodeType.BuildInsideNode;
        }

        private static void Dfs<TVertex, TEdge>(UndirectedGraph<TVertex, TEdge> graph, TVertex startVertex,
            Dictionary<TVertex, bool> vertexVisited, List<TVertex> component)
            where TEdge : IEdge<TVertex>
        {
            var stack = new Stack<TVertex>();
            stack.Push(startVertex);

            while (stack.Count > 0)
            {
                var vertex = stack.Pop();
                if (!vertexVisited[vertex])
                {
                    vertexVisited[vertex] = true;
                    component.Add(vertex);

                    foreach (var edge in graph.AdjacentEdges(vertex))
                    {
                        var nextVertex = edge.Target.Equals(vertex) ? edge.Source : edge.Target;
                        if (!vertexVisited[nextVertex]) stack.Push(nextVertex);
                    }
                }
            }
        }
    }
}