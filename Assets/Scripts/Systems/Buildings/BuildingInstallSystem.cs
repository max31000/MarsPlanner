using System;
using System.Linq;
using System.Threading.Tasks;
using Components.Buildings;
using Components.Input;
using Components.World;
using Helpers;
using Helpers.Cache;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Buildings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.Buildings
{
    public class BuildingInstallSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<BuildingAssetsComponent>> buildingAssetsFilter = null;
        private readonly EcsPoolInject<BuildingAssetsComponent> buildingAssetsPool = null;


        private readonly EcsFilterInject<Inc<BuildingsBufferComponent>> buildingsBufferFilter = null;
        private readonly EcsPoolInject<BuildingsBufferComponent> buildingsBufferPool = null;

        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> buildPlaceFilter = null;
        private readonly EcsPoolInject<PlaceBuildProcessingComponent> buildPlacePool = null;

        private readonly EcsPoolInject<BuildComponent> installedBuildPool = null;
        private readonly EcsFilterInject<Inc<BuildComponent>> installedBuildFilter = null;

        private readonly EcsFilterInject<Inc<RaycastObjectEvent>> raycastObjectEventFilter = null;
        private readonly EcsFilterInject<Inc<WorldTerrainKeeperComponent>> terrainFilter = null;

        public void Run(IEcsSystems systems)
        {
            if (buildPlaceFilter.Value.GetEntitiesCount() == 0 ||
                raycastObjectEventFilter.Value.GetEntitiesCount() == 0)
                return;

            foreach (var buildPlaceEntity in buildPlaceFilter.Value)
            {
                ref var buildingPlaceComponent = ref buildPlacePool.Value.Get(buildPlaceEntity);

                if (!buildingPlaceComponent.IsCanInstall)
                    return;

                InstantiateNewBuilding(ref buildingPlaceComponent);
            }
        }

        private void InstantiateNewBuilding(ref PlaceBuildProcessingComponent buildingPlaceComponent)
        {
            var buildingFolder = BuildingsFolderCache.GetBuildingsFolder();
            var buildingSurface = BuildingsFolderCache.GetBuildingsSurface();
            ref var buildingAssetsComponent = ref buildingAssetsPool.Value.Get(buildingAssetsFilter.Value.Single());
            var buildingAsset = buildingAssetsComponent.BuildingsAssets[buildingPlaceComponent.Type];
            var installedBuild = Object.Instantiate(
                buildingAsset,
                buildingPlaceComponent.Position,
                Quaternion.Euler(buildingPlaceComponent.Rotation),
                buildingFolder.transform
            );

            var buildComponents = installedBuild.GetComponentsInChildren<Transform>();
            var gates = buildComponents.Where(x => x.gameObject.name.Contains("GateConnection"))
                .ToArray();
            
            ref var installedBuildComponent = ref installedBuildPool.NewEntity(out var _);
            installedBuildComponent.BuildId = Guid.NewGuid();
            installedBuildComponent.Object = installedBuild;
            installedBuildComponent.Type = buildingPlaceComponent.Type;
            installedBuildComponent.BuildingGateInfos = gates.Select(x =>
            {
                var children = x.gameObject.GetComponentsInChildren<Transform>();
                var gate = children.SingleOrDefault(g => g.gameObject.name == "GateBounds");
                var outDirection = children.SingleOrDefault(g => g.gameObject.name == "OutDir");

                return new BuildingGateInfo
                {
                    Scale = gate!.lossyScale,
                    Position = gate!.position,
                    OutDirectionPosition =  outDirection!.position,
                    AttachedBuildId = null
                };
            }).ToArray();
            FlatTerrainUnderBuilding(buildingPlaceComponent.Position, installedBuild.GetComponentsInChildren<Collider>().First().bounds.size, 0.6f);
        }

        // method with build position and collider size in input for take terrain from terrainFilter, and flat terrain under building position by collider size
        private void FlatTerrainUnderBuilding(Vector3 position, Vector3 colliderSize, float additionalSpace)
        {
            ref var terrainComponent = ref terrainFilter.Pools.Inc1.Get(terrainFilter.Value.Single());
            var terrain = terrainComponent.Terrain;
            var terrainData = terrain.terrainData;
            var terrainPosition = terrain.transform.position;
            var terrainSize = terrainData.size;
            var terrainX = terrainPosition.x;
            var terrainZ = terrainPosition.z;
            var terrainWidth = terrainSize.x;
            var terrainLength = terrainSize.z;

            var colliderHalfSizeX = colliderSize.x / 2 + additionalSpace;
            var colliderHalfSizeZ = colliderSize.z / 2 + additionalSpace;
            var x1 = (position.x - colliderHalfSizeX - terrainX) / terrainWidth;
            var z1 = (position.z - colliderHalfSizeZ - terrainZ) / terrainLength;
            var x2 = (position.x + colliderHalfSizeX - terrainX) / terrainWidth;
            var z2 = (position.z + colliderHalfSizeZ - terrainZ) / terrainLength;

            var x1Index = (int) (x1 * terrainData.heightmapResolution);
            var z1Index = (int) (z1 * terrainData.heightmapResolution);
            var x2Index = (int) (x2 * terrainData.heightmapResolution);
            var z2Index = (int) (z2 * terrainData.heightmapResolution);

            var heightMap = terrainData.GetHeights(x1Index, z1Index, x2Index - x1Index, z2Index - z1Index);
            // get minimum height from 4 center elements of heightMap
            var y = FindCenterMin(heightMap);
            var newHeightMap = new float[heightMap.GetLength(0), heightMap.GetLength(1)];
            for (var i = 0; i < heightMap.GetLength(0); i++)
            for (var j = 0; j < heightMap.GetLength(1); j++)
                newHeightMap[i, j] = y;

            terrainData.SetHeights(x1Index, z1Index, newHeightMap);
        }
        
        private static float FindCenterMin(float[,] array)
        {
            var rows = array.GetLength(0);
            var cols = array.GetLength(1);
            int startRow, endRow, startCol, endCol;

            // Calculate center bounds; adjust these calculations based on your definition of "center"
            startRow = rows / 2 - 1 - (rows % 2 == 0 ? 1 : 0);
            endRow = rows / 2 + 1;
            startCol = cols / 2 - 1 - (cols % 2 == 0 ? 1 : 0);
            endCol = cols / 2 + 1;

            // Adjust bounds for smaller arrays if necessary
            startRow = Math.Max(0, startRow);
            startCol = Math.Max(0, startCol);
            endRow = Math.Min(rows, endRow);
            endCol = Math.Min(cols, endCol);

            var minVal = float.MaxValue;
            for (var i = startRow; i < endRow; i++)
            {
                for (var j = startCol; j < endCol; j++)
                {
                    minVal = Mathf.Min(array[i, j], minVal);
                }
            }
            return minVal;
        }
    }
}