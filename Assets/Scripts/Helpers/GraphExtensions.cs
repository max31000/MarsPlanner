using System.Collections.Generic;
using System.Linq;
using QuikGraph;

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