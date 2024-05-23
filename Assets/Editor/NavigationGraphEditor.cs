using System;
using System.IO;
using CustomMonoBehaviour;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LocalNavigation))]
    public class NavigationGraphEditor : UnityEditor.Editor
    {
        private const string BuildsGraphDataPath = "Assets/Resources/BuildsGraphData";
        private SerializedProperty savedDataProp;

        private void OnEnable()
        {
            savedDataProp = serializedObject.FindProperty("SavedData");
        }

        public override void OnInspectorGUI()
        {
            var navigation = (LocalNavigation)target;

            serializedObject.Update();

            // Позволяет Unity отслеживать начало изменений в инспекторе
            EditorGUI.BeginChangeCheck();

            // Рисуем стандартные поля кроме SavedData
            DrawPropertiesExcluding(serializedObject, "SavedData");

            EditorGUILayout.Space();
            DrawNodesEditor(navigation);
            EditorGUILayout.Separator();
            DrawEdgesEditor(navigation);
            EditorGUILayout.Separator();
            DrawSaveLoadButtons(navigation);

            serializedObject.ApplyModifiedProperties();
            // Проверяем, были ли внесены изменения в инспекторе
            if (EditorGUI.EndChangeCheck())
                // Если да, сохраняем изменения
                EditorUtility.SetDirty(navigation);
        }

        private void DrawSaveLoadButtons(LocalNavigation navigation)
        {
            navigation.FileName = EditorGUILayout.TextField("File name with graph data", navigation.FileName);
            EditorGUILayout.PropertyField(savedDataProp);

            if (GUILayout.Button("Save"))
                SaveGraph(navigation, $"{BuildsGraphDataPath}/{navigation.FileName}.asset");

            if (GUILayout.Button("Load"))
            {
                LoadGraph(navigation);
                EditorUtility.SetDirty(target); // Обновляем инспектор после загрузки
            }
        }

        private void SaveGraph(LocalNavigation navigation, string filePath)
        {
            EnsureFolderExists(BuildsGraphDataPath);

            var savedAsset = AssetDatabase.LoadAssetAtPath<NavigationGraphData>(filePath);

            if (savedAsset.IsUnityNull())
            {
                savedAsset = CreateNewGraphData(filePath);
                // копирование объекта через сериализацию
                savedAsset.NavigationGraph =
                    JsonUtility.FromJson<LocalNavigationGraph>(JsonUtility.ToJson(navigation.LocalGraph));
                AssetDatabase.SaveAssets();
            }

            navigation.SavedData = savedAsset;
            savedAsset.NavigationGraph =
                JsonUtility.FromJson<LocalNavigationGraph>(JsonUtility.ToJson(navigation.LocalGraph));
            EditorUtility.SetDirty(savedAsset); // Помечаем ScriptableObject как изменённый
        }

        private static void EnsureFolderExists(string buildsGraphDataPath)
        {
            var folders = buildsGraphDataPath.Split('/');
            var currentPath = "";

            foreach (var folder in folders)
            {
                if (string.IsNullOrEmpty(folder)) continue;
                if (!string.IsNullOrEmpty(currentPath)) currentPath += "/";
                currentPath += folder;

                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    var parentFolder = Path.GetDirectoryName(currentPath);
                    var newFolderName = Path.GetFileName(currentPath);
                    AssetDatabase.CreateFolder(parentFolder, newFolderName);
                }
            }
        }

        private void LoadGraph(LocalNavigation navigation)
        {
            var savedAsset = navigation.SavedData;

            if (savedAsset != null)
                navigation.LocalGraph =
                    JsonUtility.FromJson<LocalNavigationGraph>(JsonUtility.ToJson(savedAsset.NavigationGraph));
            else
                throw new Exception("No saved data found");
        }

        private NavigationGraphData CreateNewGraphData(string filePath)
        {
            var graphData = CreateInstance<NavigationGraphData>();
            AssetDatabase.CreateAsset(graphData, filePath);
            AssetDatabase.SaveAssets();
            return graphData;
        }

        private void DrawNodesEditor(LocalNavigation navigation)
        {
            if (navigation.LocalGraph.Nodes != null)
                for (var i = 0; i < navigation.LocalGraph.Nodes.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Node {i}", GUILayout.Width(50));
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

                    DrawNodeToggles(navigation, i);

                    EditorGUILayout.Space();
                }

            if (GUILayout.Button("Add Node"))
            {
                Undo.RecordObject(navigation, "Add Navigation Node");
                navigation.LocalGraph.Nodes!.Add(new UnityNavigationNode { position = Vector3.zero });
                EditorUtility.SetDirty(navigation); // Помечаем объект как изменённый
            }
        }

        private static void DrawNodeToggles(LocalNavigation navigation, int i)
        {
            EditorGUILayout.BeginHorizontal();

            // Включаем или выключаем GUI элемент в зависимости от состояния другого поля
            GUI.enabled = !navigation.LocalGraph.Nodes[i].isBuildConnectionNode;
            // Добавление галочки для IsOutNode
            var isOutNode = EditorGUILayout.Toggle("Is Out Node", navigation.LocalGraph.Nodes[i].isOutNode);
            if (isOutNode != navigation.LocalGraph.Nodes[i].isOutNode)
            {
                navigation.LocalGraph.Nodes[i].isOutNode = isOutNode;
                // Если этот параметр теперь true, сбрасываем другой параметр
                navigation.LocalGraph.Nodes[i].isBuildConnectionNode = false;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Сбрасываем ограничение активности GUI элемента для следующего элемента
            GUI.enabled = !navigation.LocalGraph.Nodes[i].isOutNode;
            // Добавление галочки для isBuildConnectionNode
            var isBuildConnectionNode = EditorGUILayout.Toggle("Connects to other buildings",
                navigation.LocalGraph.Nodes[i].isBuildConnectionNode);
            if (isBuildConnectionNode != navigation.LocalGraph.Nodes[i].isBuildConnectionNode)
            {
                navigation.LocalGraph.Nodes[i].isBuildConnectionNode = isBuildConnectionNode;
                // Если этот параметр теперь true, сбрасываем другой параметр
                navigation.LocalGraph.Nodes[i].isOutNode = false;
            }

            EditorGUILayout.EndHorizontal();
            // Восстанавливаем активность GUI элементов
            GUI.enabled = true;
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