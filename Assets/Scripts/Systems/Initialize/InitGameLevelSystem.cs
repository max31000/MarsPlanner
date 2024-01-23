using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Components;
using Components.Buildings;
using Components.Input;
using Definitions;
using Definitions.Constants;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Buildings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.Initialize
{
    public class InitGameLevelSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<BuildingAssetsComponent> buildingAssetsPool = null;
        private readonly EcsPoolInject<BuildingsBufferComponent> buildingBufferPool = null;
        private readonly EcsPoolInject<CameraComponent> cameraPool = null;
        private readonly EcsPoolInject<CubeComponent> cubePool = null;


        private readonly EcsCustomInject<GameDefinitions> definitions = default;
        private readonly EcsPoolInject<RaycastTargetComponent> raycastCoordinatesPool = null;
        private readonly EcsWorldInject world = null;
        private readonly EcsFilterInject<Inc<WorldTerrainKeeperComponent>> worldTerrainKeeperFilter = null;

        public void Init(IEcsSystems systems)
        {
            SpawnCube();
            LoadMainCamera();
            LoadBuildings();
            LoadSystems();
        }

        private void LoadMainCamera()
        {
            var cameraEntity = world.Value.NewEntity();
            ref var cameraComponent = ref cameraPool.Value.Add(cameraEntity);

            var cameraPosition = new Vector3(20, 0, 20);
            cameraComponent.Initialize(definitions.Value.CameraDefinitions,
                cameraPosition);
        }

        private void LoadSystems()
        {
            var terrain = Object.FindObjectsOfType<Terrain>().Single();
            ref var raycastCoordinatesComponent =
                ref raycastCoordinatesPool.NewEntity(out var raycastCoordinatesEntities);
            raycastCoordinatesComponent.RaycastTarget = terrain.GetComponent<Collider>();
        }

        private void SpawnCube()
        {
            var prefab = Resources.Load<GameObject>(Path.Combine(@"Prefabs/Cube"));
            var cube = Object.Instantiate(prefab);
            var cubeEntity = world.Value.NewEntity();

            ref var cubeComponent = ref cubePool.Value.Add(cubeEntity);
            cubeComponent.Cube = cube;
        }

        private void LoadBuildings()
        {
            ref var buildingAssetComponent = ref buildingAssetsPool.NewEntity(out var _);
            buildingAssetComponent.BuildingsAssets = LoadBuildingAssets();

            LoadBuildingBuffer(buildingAssetComponent.BuildingsAssets);
        }

        private void LoadBuildingBuffer(Dictionary<BuildingType, GameObject> buildingsAssets)
        {
            ref var buildingsBufferComponent = ref buildingBufferPool.NewEntity(out var _);
            buildingsBufferComponent.BuildingsBuffer = buildingsAssets
                .Select(x =>
                    (
                        Tag: x.Key,
                        Obj: Object.Instantiate(
                            x.Value,
                            BufferConstants.BufferObjectsPosition,
                            Quaternion.Euler(0, 0, 0)
                        )
                    )
                )
                // Настраиваю размеры буфферных зданий для рассчёта пересечений с ними при установке.
                .Select(x =>
                {
                    var collider = x.Obj.GetComponentInChildren<Collider>();

                    x.Obj.layer = 6;
                    collider.gameObject.layer = 6;

                    var colliderType = collider is SphereCollider ? ColliderType.Sphere : ColliderType.Box;
                    var startBounds = collider.bounds.size;

                    return (x.Tag, Buffer: new BuildingBuffer
                    {
                        InstancedBuilding = x.Obj,
                        ColliderType = colliderType,
                        StartBoundSize = startBounds
                    });
                })
                .ToDictionary(x => x.Tag, x => x.Buffer);
        }

        private static Dictionary<BuildingType, GameObject> LoadBuildingAssets()
        {
            var assets = new Dictionary<BuildingType, GameObject>();

            foreach (BuildingType buildType in Enum.GetValues(typeof(BuildingType)))
            {
                var type = buildType.ToString();
                var path = @"Prefabs/Buildings/" + type;

                assets.Add(buildType, Resources.Load<GameObject>(path));
            }

            return assets;
        }
    }
}