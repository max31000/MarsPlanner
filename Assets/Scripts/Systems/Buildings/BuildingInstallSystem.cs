using Components.Buildings;
using Components.Input;
using Definitions.Constants;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

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

        private readonly EcsFilterInject<Inc<RaycastObjectEvent>> raycastObjectEventFilter = null;


        public void Run(IEcsSystems systems)
        {
            if (buildPlaceFilter.Value.GetEntitiesCount() == 0 ||
                raycastObjectEventFilter.Value.GetEntitiesCount() == 0)
                return;

            ref var buildingPlaceComponent = ref buildPlacePool.Value.Get(buildPlaceFilter.Value.Single());

            InstantiateNewBuilding(ref buildingPlaceComponent);
            ResetBufferObjectPosition(ref buildingPlaceComponent);
        }

        private void InstantiateNewBuilding(ref PlaceBuildProcessingComponent buildingPlaceComponent)
        {
            ref var buildingAssetsComponent = ref buildingAssetsPool.Value.Get(buildingAssetsFilter.Value.Single());
            var buildingAsset = buildingAssetsComponent.BuildingsAssets[buildingPlaceComponent.Type];
            Object.Instantiate(
                buildingAsset,
                buildingPlaceComponent.Position,
                Quaternion.Euler(buildingPlaceComponent.Rotation)
            );
        }

        private void ResetBufferObjectPosition(ref PlaceBuildProcessingComponent buildingPlaceComponent)
        {
            ref var buildingBuffer = ref buildingsBufferPool.Value.Get(buildingsBufferFilter.Value.Single());
            var buildingBufferObject = buildingBuffer.BuildingsBuffer[buildingPlaceComponent.Type].InstancedBuilding;
            buildingBufferObject.transform.position = BufferConstants.BufferObjectsPosition;
        }
    }
}