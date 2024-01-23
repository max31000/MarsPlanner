using System.Linq;
using Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems.Initialize
{
    public class TerrainInitializeSystem : IEcsPreInitSystem
    {
        private readonly EcsPoolInject<WorldTerrainKeeperComponent> worldTerrainKeeperPool = null;

        public void PreInit(IEcsSystems systems)
        {
            // отключаю генерацию на время разработки
            /*NavMeshBuilder.ClearAllNavMeshes();
            NavMeshBuilder.BuildNavMesh();
            var generator = new HeightMapGenerator();
            var heightMap = generator.Generate();*/

            var terrain = Object.FindObjectsOfType<Terrain>().Single();
            //terrain.terrainData.SetHeights(0, 0, heightMap);

            ref var terrainKeeperComponent = ref worldTerrainKeeperPool.NewEntity(out _);
            terrainKeeperComponent.Terrain = terrain;

            //SetTextures(heightMap, terrain);
        }

        private static void SetTextures(float[,] heightMap, Terrain terrain)
        {
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            var textureMap = new float[width - 1, height - 1, 3];
            for (var x = 0; x < width - 1; x++)
            for (var y = 0; y < height - 1; y++)
            {
                var terrainHeight = heightMap[x, y];
                if (terrainHeight <= 0.001f)
                {
                    textureMap[x, y, 0] = 0.8f;
                    textureMap[x, y, 1] = 0.2f;
                }
                else
                {
                    textureMap[x, y, 0] = 0.5f;
                    textureMap[x, y, 1] = 0.5f;
                }
            }

            terrain.terrainData.SetAlphamaps(0, 0, textureMap);
        }
    }
}