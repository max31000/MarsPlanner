using System.Collections.Generic;
using System.Linq;
using Helpers;
using Priority_Queue;
using QuikGraph;
using RBush;
using UnityEngine;

namespace Models.Navigation
{
    public class NavigationSystem : INavigationSystem
    {
        public NavigationSystem(Terrain terrain, float cellSize, float maxNavigationHeight, int maxNormalAngle)
        {
            NavigationGrid = GenerateNavigationGrid(cellSize, terrain, maxNavigationHeight, maxNormalAngle);
            NavigationGrid.RemoveDisconnectedSubgraphs();
            PointsIndex = GetSpatialIndex();
        }

        private UndirectedGraph<NavigationNode, Edge<NavigationNode>> NavigationGrid { get; }
        private RBush<PointWithNode> PointsIndex { get; }

        public NavigationNode FindClosestNode(Vector3 point, NodeType? nodeType = null, NodeStatus? nodeStatus = null)
        {
            return PointsIndex.Knn(1, point.x, point.z,
                predicate: p =>
                    (nodeType == null || p.Node.Type == nodeType) && (nodeStatus == null || p.Node.Status == nodeStatus)
            ).First().Node;
        }

        public IEnumerable<NavigationNode> GetNodesInArea(Vector3 point, int width, int height, float rotationDegrees)
        {
            var rad = rotationDegrees * Mathf.Deg2Rad;
            var cos = Mathf.Cos(rad);
            var sin = Mathf.Sin(rad);

            var corners = new Vector3[4];
            corners[0] = new Vector3(-width / 2.0f, 0, -height / 2.0f);
            corners[1] = new Vector3(width / 2.0f, 0, -height / 2.0f);
            corners[2] = new Vector3(width / 2.0f, 0, height / 2.0f);
            corners[3] = new Vector3(-width / 2.0f, 0, height / 2.0f);

            // Поворот углов
            for (var i = 0; i < 4; i++)
            {
                var rotatedX = corners[i].x * cos - corners[i].z * sin;
                var rotatedZ = corners[i].x * sin + corners[i].z * cos;
                corners[i] = new Vector3(rotatedX, 0, rotatedZ) + point;
            }

            var minX = corners.Min(c => c.x);
            var maxX = corners.Max(c => c.x);
            var minY = corners.Min(c => c.z);
            var maxY = corners.Max(c => c.z);

            Debug.DrawLine(new Vector3(minX, 0, (float)minY), new Vector3(minX, 0, (float)maxY),
                Color.white, 10000, true);
            Debug.DrawLine(new Vector3(minX, 0, (float)maxY), new Vector3(maxX, 0, (float)maxY),
                Color.white, 10000, true);
            Debug.DrawLine(new Vector3(maxX, 0, (float)maxY), new Vector3(maxX, 0, (float)minY),
                Color.white, 10000, true);
            Debug.DrawLine(new Vector3(maxX, 0, (float)minY), new Vector3(minX, 0, (float)minY),
                Color.white, 10000, true);

            return PointsIndex
                .Knn(1000, point.x, point.z,
                    predicate: p => minX <= p.X && p.X <= maxX && minY <= p.Y && p.Y <= maxY)
                .Select(p => p.Node);
        }

        public void AddSubgraph(UndirectedGraph<NavigationNode, Edge<NavigationNode>> subgraph,
            NavigationNode subGraphConnectingNode, NavigationNode otherConnectingNode)
        {
            NavigationGrid.AddVertexRange(subgraph.Vertices);
            NavigationGrid.AddEdgeRange(subgraph.Edges);

            if (otherConnectingNode != null && subGraphConnectingNode != null)
                NavigationGrid.AddEdge(new Edge<NavigationNode>(otherConnectingNode, subGraphConnectingNode));

            foreach (var point in subgraph.Vertices.Select(node => new PointWithNode(node))) PointsIndex.Insert(point);
        }

        // find path with A* algorithm
        public List<NavigationNode> FindPathTo(Vector3 startPoint, Vector3 destination)
        {
            var startNode = FindClosestNode(startPoint);
            var destinationNode = FindClosestNode(destination);

            var openSet = new SimplePriorityQueue<NavigationNode, float>();
            var cameFrom = new Dictionary<NavigationNode, NavigationNode>();

            var gScore = new Dictionary<NavigationNode, float>();
            var fScore = new Dictionary<NavigationNode, float>();

            foreach (var node in NavigationGrid.Vertices)
            {
                gScore[node] = float.MaxValue;
                fScore[node] = float.MaxValue;
            }

            gScore[startNode] = 0f;
            fScore[startNode] = startNode.Distance(destinationNode);

            openSet.Enqueue(startNode, fScore[startNode]);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.Dequeue();

                if (currentNode == destinationNode)
                    return ReconstructPath(cameFrom, currentNode);

                foreach (var edge in NavigationGrid.AdjacentEdges(currentNode))
                {
                    var neighbor = edge.Target.Equals(currentNode) ? edge.Source : edge.Target;
                    var tentativeGScore = gScore[currentNode] + currentNode.Distance(neighbor);

                    if (tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = currentNode;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + neighbor.Distance(destinationNode);
                        if (!openSet.Contains(neighbor) && neighbor.Status != NodeStatus.Disabled)
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }

            return new List<NavigationNode>(); // return an empty path if no path is found
        }

        // Метод для визуализации графа
        public void DrawGraph()
        {
            DrawEdges(1000);
            DrawNodes(1000);
        }

        private List<NavigationNode> ReconstructPath(Dictionary<NavigationNode, NavigationNode> cameFrom,
            NavigationNode current)
        {
            var totalPath = new List<NavigationNode> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }

            return totalPath;
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

        private void DrawEdges(int duration)
        {
            var drawnEdges = new HashSet<string>();

            foreach (var edge in NavigationGrid.Edges)
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
                Debug.DrawLine(edgeSourcePosition, edgeTargetPosition, Color.red, duration, true);
            }
        }

        private void DrawNodes(int duration)
        {
            var drawnNodes = new HashSet<string>();

            foreach (var vertex in NavigationGrid.Vertices)
            {
                if (drawnNodes.Contains(GetNavigationNodeKey(vertex.Position)))
                    continue;

                drawnNodes.Add(GetNavigationNodeKey(vertex.Position));
                DrawNode(vertex.Position, duration);
            }
        }

        // Метод для рисования ноды
        private static void DrawNode(Vector3 position, int duration)
        {
            var size = 0.1f; // Размер "маркера" ноды
            Debug.DrawLine(position + Vector3.up * size, position - Vector3.up * size, Color.green, duration, false);
            Debug.DrawLine(position + Vector3.right * size, position - Vector3.right * size, Color.green, duration,
                false);
            Debug.DrawLine(position + Vector3.forward * size, position - Vector3.forward * size, Color.green, duration,
                false);
        }

        private RBush<PointWithNode> GetSpatialIndex()
        {
            var index = new RBush<PointWithNode>();
            foreach (var vertex in NavigationGrid.Vertices)
            {
                var pointWithNode = new PointWithNode(vertex);
                index.Insert(pointWithNode);
            }

            return index;
        }
    }
}