using CustomMonoBehaviour;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LocalNavigation))]
    public class NavigationGraphEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var navigation = (LocalNavigation)target;

            // Позволяет Unity отслеживать начало изменений в инспекторе
            EditorGUI.BeginChangeCheck();

            // Отрисовка стандартных полей
            DrawDefaultInspector();

            EditorGUILayout.Space();
            DrawNodesEditor(navigation);
            EditorGUILayout.Separator();
            DrawEdgesEditor(navigation);
            EditorGUILayout.Separator();
            DrawSaveLoadButtons(navigation);

            // Проверяем, были ли внесены изменения в инспекторе
            if (EditorGUI.EndChangeCheck())
                // Если да, сохраняем изменения
                EditorUtility.SetDirty(navigation);
        }

        private void DrawSaveLoadButtons(LocalNavigation navigation)
        {
            if (GUILayout.Button("Save"))
                SaveGraph(navigation);

            if (GUILayout.Button("Load"))
            {
                LoadGraph(navigation);
                EditorUtility.SetDirty(target); // Обновляем инспектор после загрузки
            }
        }

        private void SaveGraph(LocalNavigation navigation)
        {
            var savedAsset = AssetDatabase.LoadAssetAtPath<NavigationGraphData>(
                "Assets/NavigationGraphData.asset");

            if (savedAsset.IsUnityNull())
            {
                savedAsset = CreateNewGraphData();
                // копирование объекта через сериализацию
                savedAsset.NavigationGraph =
                    JsonUtility.FromJson<LocalNavigationGraph>(JsonUtility.ToJson(navigation.LocalGraph));
                AssetDatabase.SaveAssets();
            }

            savedAsset.NavigationGraph =
                JsonUtility.FromJson<LocalNavigationGraph>(JsonUtility.ToJson(navigation.LocalGraph));
            EditorUtility.SetDirty(savedAsset); // Помечаем ScriptableObject как изменённый
        }

        private void LoadGraph(LocalNavigation navigation)
        {
            var savedAsset = AssetDatabase.LoadAssetAtPath<NavigationGraphData>(
                "Assets/NavigationGraphData.asset");

            if (savedAsset != null)
                navigation.LocalGraph =
                    JsonUtility.FromJson<LocalNavigationGraph>(JsonUtility.ToJson(savedAsset.NavigationGraph));
        }

        private NavigationGraphData CreateNewGraphData()
        {
            var graphData = CreateInstance<NavigationGraphData>();
            AssetDatabase.CreateAsset(graphData, "Assets/NavigationGraphData.asset");
            AssetDatabase.SaveAssets();
            return graphData;
        }

        private void DrawNodesEditor(LocalNavigation navigation)
        {
            if (navigation.LocalGraph.Nodes != null)
                for (var i = 0; i < navigation.LocalGraph.Nodes.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Node {i + 1}", GUILayout.Width(50));
                    navigation.LocalGraph.Nodes[i].position =
                        EditorGUILayout.Vector3Field("", navigation.LocalGraph.Nodes[i].position);

                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        Undo.RecordObject(navigation, "Remove Navigation Node");
                        navigation.LocalGraph.Nodes.RemoveAt(i);
                        EditorUtility.SetDirty(navigation); // Помечаем объект как изменённый
                        break; // Выходим из цикла, так как коллекция изменилась
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();

                    // Добавление галочки для IsOutNode
                    navigation.LocalGraph.Nodes[i].isOutNode =
                        EditorGUILayout.Toggle("Is Out Node", navigation.LocalGraph.Nodes[i].isOutNode);
                    
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    
                    // Добавление галочки для isBuildConnectionNode
                    navigation.LocalGraph.Nodes[i].isBuildConnectionNode =
                        EditorGUILayout.Toggle("Connects to other buildings", navigation.LocalGraph.Nodes[i].isBuildConnectionNode);

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }

            if (GUILayout.Button("Add Node"))
            {
                Undo.RecordObject(navigation, "Add Navigation Node");
                navigation.LocalGraph.Nodes!.Add(new UnityNavigationNode { position = Vector3.zero });
                EditorUtility.SetDirty(navigation); // Помечаем объект как изменённый
            }
        }

        private void DrawEdgesEditor(LocalNavigation navigation)
        {
            for (var i = 0; i < navigation.LocalGraph.Edges.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Edge {i}", GUILayout.Width(50));
                var edge = navigation.LocalGraph.Edges[i];
                edge.startIndex = Mathf.Max(EditorGUILayout.IntField(edge.startIndex, GUILayout.Width(50)), 0);
                edge.endIndex = Mathf.Max(EditorGUILayout.IntField(edge.endIndex, GUILayout.Width(50)), 0);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Undo.RecordObject(navigation, "Remove Navigation Edge");
                    navigation.LocalGraph.Edges.RemoveAt(i);
                    break; // Выходим, так как список изменился
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Edge"))
            {
                Undo.RecordObject(navigation, "Add Navigation Edge");
                if (navigation.LocalGraph.Nodes.Count > 1) // Проверяем, что есть минимум две вершины
                    navigation.LocalGraph.Edges.Add(new UnityNavigationEdge { startIndex = 0, endIndex = 1 });
            }
        }
    }
}