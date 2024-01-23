using Components.Buildings;
using Components.Input;
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

        private readonly EcsPoolInject<BuildComponent> installedBuildPool = null;


        private readonly EcsFilterInject<Inc<RaycastObjectEvent>> raycastObjectEventFilter = null;


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
            ref var buildingAssetsComponent = ref buildingAssetsPool.Value.Get(buildingAssetsFilter.Value.Single());
            var buildingAsset = buildingAssetsComponent.BuildingsAssets[buildingPlaceComponent.Type];
            var installedBuild = Object.Instantiate(
                buildingAsset,
                buildingPlaceComponent.Position,
                Quaternion.Euler(buildingPlaceComponent.Rotation)
            );

            ref var installedBuildComponent = ref installedBuildPool.NewEntity(out var _);
            installedBuildComponent.Object = installedBuild;
            installedBuildComponent.Type = buildingPlaceComponent.Type;
        }
    }
}