using System.Linq;
using Helpers;
using Leopotam.EcsLite;
using UnityEditor.AI;
using UnityEngine;

namespace Systems.Initialize
{
    public class TerrainInitializeSystem : IEcsPreInitSystem
    {
        public void PreInit(IEcsSystems systems)
        {
            //NavMeshBuilder.ClearAllNavMeshes();
            //NavMeshBuilder.BuildNavMesh();
            var generator = new HeightMapGenerator();
            var heightMap = generator.Generate();
            var terrain = Object.FindObjectsOfType<Terrain>();
            terrain.Single().terrainData.SetHeights(0, 0, heightMap);
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            var textureMap = new float[width - 1, height - 1, 3];
            for (var x = 0; x < width - 1; x++)
            for (var y = 0; y < height - 1; y++)
            {
                var terrainHeight = heightMap[x, y];
                var layer = 0;
                if (terrainHeight <= 0.001f)
                {
                    layer = 1;
                }

                if (terrainHeight >= 0.1f)
                {
                    layer = 2;
                }

                textureMap[x, y, layer] = 1;
            }
            terrain.Single().terrainData.SetAlphamaps(0, 0, textureMap);
        }
    }
}