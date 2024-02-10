using System.Collections.Generic;
using System.Linq;
using Components.World;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Navigation;
using QuikGraph;
using UnityEngine;

namespace Systems.Initialize
{
    public class NavigationGraphInitializeSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<WorldTerrainKeeperComponent>> worldTerrainFilter = null;

        public void Init(IEcsSystems systems)
        {
            foreach (var terrainEntity in worldTerrainFilter.Value)
            {
                ref var terrainComponent = ref worldTerrainFilter.Pools.Inc1.Get(terrainEntity);
                var graph = GenerateNavigationGrid(3, terrainComponent.Terrain, 10, 30);
                graph.RemoveDisconnectedSubgraphs();
                DrawGraph(graph);
            }
        }

        private static UndirectedGraph<NavigationNode, Edge<NavigationNode>> GenerateNavigationGrid(float cellSize,
            Terrain terrain, float maxNavigationHeight,
            int maxNormalAngle)
        {
            if (terrain == null) return null;

            var nodesByPosition = new Dictionary<string, NavigationNode>();

            var graph = CreateGraphWithAllNodes(cellSize, terrain, maxNavigationHeight, maxNormalAngle,
                nodesByPosition);
            AddAllEdgesToGraph(graph, nodesByPosition, cellSize);

            return graph;
        }

        private static UndirectedGraph<NavigationNode, Edge<NavigationNode>> CreateGraphWithAllNodes(float cellSize,
            Terrain terrain, float maxNavigationHeight, int maxNormalAngle,
            Dictionary<string, NavigationNode> nodesByPosition)
        {
            var graph = new UndirectedGraph<NavigationNode, Edge<NavigationNode>>(false);

            var terrainData = terrain.terrainData;
            var width = (int)(terrainData.size.x / cellSize);
            var length = (int)(terrainData.size.z / cellSize);

            for (var x = 0; x <= width; x++)
            for (var z = 0; z <= length; z++)
            {
                var cornerStartPosition = terrain.transform.position + new Vector3(x * cellSize, 0, z * cellSize);
                var height = terrain.SampleHeight(cornerStartPosition);
                cornerStartPosition.y = height;
                var normal = terrainData.GetInterpolatedNormal(x / (float)width, z / (float)length);

                if (height <= maxNavigationHeight && Vector3.Angle(normal, Vector3.up) <= maxNormalAngle)
                    // Добавляем узлы в граф
                    AddNodesToGraph(graph, cellSize, cornerStartPosition, nodesByPosition);
            }

            return graph;
        }

        private static void AddNodesToGraph(IMutableVertexAndEdgeSet<NavigationNode, Edge<NavigationNode>> graph,
            float cellSize,
            Vector3 cornerStartPosition, Dictionary<string, NavigationNode> nodesByPosition)
        {
            // Проверка и создание узла для каждого угла, если он ещё не создан
            for (var i = 0; i <= 1; i++)
            for (var j = 0; j <= 1; j++)
            {
                var cornerPosition = new Vector3(cornerStartPosition.x + i * cellSize, cornerStartPosition.y,
                    cornerStartPosition.z + j * cellSize);
                var nodeKey = GetNavigationNodeKey(cornerPosition);
                if (!nodesByPosition.ContainsKey(nodeKey))
                {
                    var node = new NavigationNode(cornerPosition);
                    graph.AddVertex(node);
                    nodesByPosition[nodeKey] = node;
                }
            }
        }

        private static void AddAllEdgesToGraph(IMutableEdgeListGraph<NavigationNode, Edge<NavigationNode>> graph,
            Dictionary<string, NavigationNode> nodesByPosition, float cellSize)
        {
            foreach (var node in graph.Vertices) AddEdges(graph, node, nodesByPosition, cellSize);
        }

        private static void AddEdges(IMutableEdgeListGraph<NavigationNode, Edge<NavigationNode>> graph,
            NavigationNode node,
            Dictionary<string, NavigationNode> nodesByPosition,
            float cellSize)
        {
            Vector3[] directions =
            {
                new(cellSize, 0, 0), // вправо
                new(0, 0, cellSize), // вверх
                new(cellSize, 0, cellSize), // диагональ вправо-вверх
                new(cellSize, 0, -cellSize) // диагональ влево-вверх
            };

            foreach (var dir in directions)
            {
                var neighbourPos = node.Position + dir;
                var neighbourKey = GetNavigationNodeKey(neighbourPos);
                if (nodesByPosition.TryGetValue(neighbourKey, out var createdNode))
                {
                    var edge = new Edge<NavigationNode>(node, createdNode);
                    graph.AddEdge(edge);
                }
            }
        }

        private static string GetNavigationNodeKey(Vector3 position)
        {
            return $"{position.x}_{position.z}";
        }

        private static string GetNavigationEdgeKey(Vector3 sourcePosition, Vector3 targetPosition)
        {
            return $"{GetNavigationNodeKey(sourcePosition)}_{GetNavigationNodeKey(targetPosition)}";
        }

        // Метод для визуализации графа
        private static void DrawGraph(IEdgeListGraph<NavigationNode, Edge<NavigationNode>> graph)
        {
            DrawEdges(graph);
            DrawNodes(graph);
        }

        private static void DrawEdges(IEdgeSet<NavigationNode, Edge<NavigationNode>> graph)
        {
            var drawnEdges = new HashSet<string>();

            foreach (var edge in graph.Edges)
            {
                var key = GetNavigationEdgeKey(edge.Source.Position, edge.Target.Position);
                var reverseKey = GetNavigationEdgeKey(edge.Target.Position, edge.Source.Position);

                if (drawnEdges.Contains(key) || drawnEdges.Contains(reverseKey))
                    continue;

                drawnEdges.Add(key);
                drawnEdges.Add(reverseKey);

                var edgeSourcePosition = new Vector3(edge.Source.Position.x, edge.Source.Position.y + 0.1f,
                    edge.Source.Position.z);
                var edgeTargetPosition = new Vector3(edge.Target.Position.x, edge.Target.Position.y + 0.1f,
                    edge.Target.Position.z);
                Debug.DrawLine(edgeSourcePosition, edgeTargetPosition, Color.red, 10000, true);
            }
        }

        private static void DrawNodes(IVertexSet<NavigationNode> graph)
        {
            var drawnNodes = new HashSet<string>();

            foreach (var vertex in graph.Vertices)
            {
                if (drawnNodes.Contains(GetNavigationNodeKey(vertex.Position))) 
                    continue;
                
                drawnNodes.Add(GetNavigationNodeKey(vertex.Position));
                DrawNode(vertex.Position);
            }
        }

        // Метод для рисования ноды
        private static void DrawNode(Vector3 position)
        {
            var size = 0.1f; // Размер "маркера" ноды
            Debug.DrawLine(position + Vector3.up * size, position - Vector3.up * size, Color.green, 10000, false);
            Debug.DrawLine(position + Vector3.right * size, position - Vector3.right * size, Color.green, 10000, false);
            Debug.DrawLine(position + Vector3.forward * size, position - Vector3.forward * size, Color.green, 10000, false);
        }
    }
}