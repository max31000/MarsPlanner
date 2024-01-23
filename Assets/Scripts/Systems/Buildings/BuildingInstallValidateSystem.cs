using System;
using Components.Buildings;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Buildings;
using UnityEngine;

namespace Systems.Buildings
{
    public class BuildingInstallValidateSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<BuildingsBufferComponent>> buildBufferFilter = null;
        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> buildPlaceFilter = null;
        private readonly EcsPoolInject<PlaceBuildProcessingComponent> buildPlacePool = null;

        public void Run(IEcsSystems systems)
        {
            foreach (var buildPlaceEntity in buildPlaceFilter.Value)
            {
                ref var buildingPlaceComponent = ref buildPlacePool.Value.Get(buildPlaceEntity);

                ref var buildBufferComponent = ref buildBufferFilter.Pools.Inc1.Get(buildBufferFilter.Value.Single());

                buildingPlaceComponent.IsCanInstall = !HasBuildIntersects(
                    buildingPlaceComponent.Position,
                    buildingPlaceComponent.Rotation,
                    buildBufferComponent.BuildingsBuffer[buildingPlaceComponent.Type]
                );
            }
        }

        private static bool HasBuildIntersects(Vector3 position, Vector3 rotation, BuildingBuffer buildingBuffer)
        {
            var layerMask = LayerMask.GetMask("Default");
            return buildingBuffer.ColliderType switch
            {
                ColliderType.Sphere => Physics.CheckSphere(position, buildingBuffer.StartBoundSize.x / 2, layerMask),
                ColliderType.Box => Physics.CheckBox(position, buildingBuffer.StartBoundSize / 2,
                    Quaternion.Euler(rotation), layerMask),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}