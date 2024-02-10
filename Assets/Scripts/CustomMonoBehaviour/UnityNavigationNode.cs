using System;
using UnityEngine;

namespace CustomMonoBehaviour
{
    [Serializable]
    public class UnityNavigationNode
    {
        public Vector3 position;

        // нужно для соединения с нодами других зданий
        public bool isBuildConnectionNode;

        // нужно для соединения с нодами внешней сетки
        public bool isOutNode;
    }
}