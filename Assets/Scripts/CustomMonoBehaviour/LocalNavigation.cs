using UnityEngine;

namespace CustomMonoBehaviour
{
    public class LocalNavigation : MonoBehaviour
    {
        public LocalNavigationGraph LocalGraph = new();

        private void OnDrawGizmos()
        {
            if (LocalGraph.Nodes != null)
            {
                foreach (var node in LocalGraph.Nodes)
                {
                    Gizmos.color = Color.red; // Установите цвет Gizmo
                    var globalPosition =
                        transform.TransformPoint(node.position); // Преобразование локальных координат в глобальные
                    Gizmos.DrawSphere(globalPosition, 0.1f); // Рисуем сферу для каждой ноды
                }

                // Для визуализации рёбер можно добавить следующий код
                Gizmos.color = Color.black; // Цвет линий для рёбер
                foreach (var edge in LocalGraph.Edges)
                    if (edge.startIndex >= 0 && edge.startIndex < LocalGraph.Nodes.Count && edge.endIndex >= 0 &&
                        edge.endIndex < LocalGraph.Nodes.Count)
                    {
                        var startNode = LocalGraph.Nodes[edge.startIndex];
                        var endNode = LocalGraph.Nodes[edge.endIndex];
                        var startGlobalPosition = transform.TransformPoint(startNode.position);
                        var endGlobalPosition = transform.TransformPoint(endNode.position);
                        Gizmos.DrawLine(startGlobalPosition, endGlobalPosition); // Рисуем линии между нодами
                    }
            }
        }
    }
}