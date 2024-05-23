using UnityEngine;

namespace CustomMonoBehaviour
{
    public class LocalNavigation : MonoBehaviour
    {
        public NavigationGraphData SavedData;

        // скрываем поля, так как их редактирование возможно через кастомные поля в NavigationGraphEditor
        [HideInInspector] public LocalNavigationGraph LocalGraph = new();
        [HideInInspector] public string FileName = "GraphData";

        private void OnDrawGizmos()
        {
            if (LocalGraph.Nodes == null) 
                return;
            for (var i = 0; i < LocalGraph.Nodes.Count; i++)
            {
                var node = LocalGraph.Nodes[i];
                Gizmos.color = GetNodeColor(node); // Установите цвет Gizmo
                var globalPosition =
                    transform.TransformPoint(node.position); // Преобразование локальных координат в глобальные
                Gizmos.DrawSphere(globalPosition, 0.1f); // Рисуем сферу для каждой ноды

#if UNITY_EDITOR
                var textStyle = new GUIStyle
                {
                    fontSize = 20,
                    normal =
                    {
                        textColor = Gizmos.color
                    }
                };
                // Отображение индекса над нодой
                UnityEditor.Handles.Label(globalPosition + Vector3.up * 0.5f, i.ToString(), textStyle);
#endif
            }

            // Визуализация рёбер
            Gizmos.color = Color.red;
            foreach (var edge in LocalGraph.Edges)
                if (edge.startIndex >= 0 && edge.startIndex < LocalGraph.Nodes.Count && edge.endIndex >= 0 &&
                    edge.endIndex < LocalGraph.Nodes.Count)
                {
                    var startNode = LocalGraph.Nodes[edge.startIndex];
                    var endNode = LocalGraph.Nodes[edge.endIndex];
                    var startGlobalPosition = transform.TransformPoint(startNode.position);
                    var endGlobalPosition = transform.TransformPoint(endNode.position);
                    Gizmos.DrawLine(startGlobalPosition, endGlobalPosition);
                }
        }

        private static Color GetNodeColor(UnityNavigationNode node)
        {
            if (node.isBuildConnectionNode)
                return Color.blue;
            if (node.isOutNode)
                return Color.magenta;
            return Color.white;
        }
    }
}