using System.IO;
using Components;
using Definitions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class InitWorldSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<GameDefinitions> definitions = default;
        private readonly EcsPoolInject<CameraComponent> cameraPool = null;
        private readonly EcsPoolInject<CubeComponent> cubePool = null;
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

            cameraComponent.maxHeigth = definitions.Value.CameraDefinitions.maxHeigth;
            cameraComponent.minHeigth = definitions.Value.CameraDefinitions.minHeigth;
            cameraComponent.rotateSpeed = definitions.Value.CameraDefinitions.rotateSpeed;
            cameraComponent.speed = definitions.Value.CameraDefinitions.speed;
            cameraComponent.verticalMoveSpeed = definitions.Value.CameraDefinitions.verticalMoveSpeed;
            cameraComponent.verticalMoveStepFactor = definitions.Value.CameraDefinitions.verticalMoveStepFactor;

            cameraComponent.Camera = Object.Instantiate(definitions.Value.CameraDefinitions.mainCameraPrefab);

            cameraComponent.minX = definitions.Value.CameraDefinitions.minX;
            cameraComponent.maxX = definitions.Value.CameraDefinitions.maxX;
            cameraComponent.minZ = definitions.Value.CameraDefinitions.minZ;
            cameraComponent.maxZ = definitions.Value.CameraDefinitions.maxZ;

            // ReSharper disable once Unity.InefficientPropertyAccess
            cameraComponent.Camera.transform.position =
                new Vector3(20, cameraComponent.Camera.transform.position.y, 20);
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