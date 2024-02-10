using UnityEngine;

namespace Helpers.Cache
{
    public static class BuildingsFolderCache
    {
        private static GameObject buildingFolder;
        private static GameObject buildingsSurface;
        
        public static GameObject GetBuildingsFolder()
        {
            if (buildingFolder == null)
            {
                CreateObjects();
            }

            return buildingFolder;
        }

        public static GameObject GetBuildingsSurface()
        {
            if (buildingsSurface == null)
            {
                CreateObjects();
            }

            return buildingsSurface;
        }

        private static void CreateObjects()
        {
            buildingFolder = new GameObject("BuildingFolder");
            buildingsSurface = Object.Instantiate(Resources.Load<GameObject>("Prefabs/BuildingsSurface"), buildingFolder.transform, true);
        }
    }
}