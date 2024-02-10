using UnityEngine;

namespace CustomMonoBehaviour
{
    [CreateAssetMenu(fileName = "NavigationGraphData", menuName = "Navigation/GraphData", order = 1)]
    public class NavigationGraphData : ScriptableObject
    {
        public LocalNavigationGraph NavigationGraph;
    }
}