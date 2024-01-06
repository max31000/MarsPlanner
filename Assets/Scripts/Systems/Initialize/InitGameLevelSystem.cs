using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Components;
using Components.Buildings;
using Components.Input;
using Definitions;
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

            cameraComponent.Initialize(definitions.Value.CameraDefinitions,
                new Vector3(20, 0, 20));
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
            ref var buildingAssetComponent = ref buildingAssetsPool.NewEntity(out var buildingAssetEntity);

            buildingAssetComponent.BuildingsAssets = LoadBuildingAssets();

            LoadBuildingBuffer(buildingAssetComponent.BuildingsAssets);
        }

        private void LoadBuildingBuffer(Dictionary<BuildingTypes, GameObject> buildingsAssets)
        {
            ref var buildingsBufferComponent = ref buildingBufferPool.NewEntity(out var _);
            buildingsBufferComponent.BuildingsBuffer = buildingsAssets
                .Select(x =>
                    (
                        Tag: x.Key,
                        Obj: Object.Instantiate(
                            x.Value,
                            new Vector3(0, -500, 0),
                            Quaternion.Euler(0, 0, 0)
                        )
                    )
                )
                .ToDictionary(x => x.Tag, x => new BuildingBuffer { InstancedBuilding = x.Obj });
        }

        private static Dictionary<BuildingTypes, GameObject> LoadBuildingAssets()
        {
            var assets = new Dictionary<BuildingTypes, GameObject>();

            foreach (BuildingTypes buildType in Enum.GetValues(typeof(BuildingTypes)))
            {
                var type = buildType.ToString();
                var path = @"Prefabs/Buildings/" + type;

                assets.Add(buildType, Resources.Load<GameObject>(path));
            }

            return assets;
        }
    }
}