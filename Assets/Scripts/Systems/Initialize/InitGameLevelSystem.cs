using System.IO;
using Components;
using Definitions;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class InitGameLevelSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<CameraComponent> cameraPool = null;
        private readonly EcsPoolInject<CubeComponent> cubePool = null;
        private readonly EcsCustomInject<GameDefinitions> definitions = default;
        private readonly EcsWorldInject world = null;

        public void Init(IEcsSystems systems)
        {
            SpawnCube();
            LoadMainCamera();
        }

        private void LoadMainCamera()
        {
            var cameraEntity = world.Value.NewEntity();
            ref var cameraComponent = ref cameraPool.Value.Add(cameraEntity);

            cameraComponent.Initialize(definitions.Value.CameraDefinitions,
                new Vector3(20, 0, 20));
        }

        private void SpawnCube()
        {
            var prefab = Resources.Load<GameObject>(Path.Combine(@"Prefabs/Cube"));
            var cube = Object.Instantiate(prefab);
            var cubeEntity = world.Value.NewEntity();

            ref var cubeComponent = ref cubePool.Value.Add(cubeEntity);
            cubeComponent.Cube = cube;
        }
    }
}