using System.Collections.Generic;
using Components.Buildings;
using Helpers.Cache;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Buildings;
using UnityEngine;

namespace Systems.Buildings
{
    public class BuildingMaterialSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<BuildingsBufferComponent>> buildingsBufferFilter = default;
        private readonly EcsPoolInject<BuildingsBufferComponent> buildingsBufferPool = default;
        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> placeBuildFilter = default;

        private readonly EcsPoolInject<PlaceBuildProcessingComponent> placeBuildPool = default;

        public void Run(IEcsSystems systems)
        {
            var buildingInstallStatus = new Dictionary<BuildingType, bool>();

            foreach (var placeEntity in placeBuildFilter.Value)
            {
                ref var placeBuildComponent = ref placeBuildPool.Value.Get(placeEntity);
                buildingInstallStatus[placeBuildComponent.Type] = placeBuildComponent.IsCanInstall;
            }

            foreach (var bufferEntity in buildingsBufferFilter.Value)
            {
                ref var buildingsBufferComponent = ref buildingsBufferPool.Value.Get(bufferEntity);

                foreach (var pair in buildingsBufferComponent.BuildingsBuffer)
                    if (buildingInstallStatus.TryGetValue(pair.Key, out var isCanInstall))
                    {
                        var buildingObject = pair.Value.InstancedBuilding;
                        var meshRenderer = buildingObject.GetComponentInChildren<MeshRenderer>();

                        if (meshRenderer != null && meshRenderer.materials.Length > 1)
                        {
                            var materials = meshRenderer.materials;
                            materials[1] = isCanInstall
                                ? MaterialCache.GetOutlineGreen()
                                : MaterialCache.GetOutlineRed();
                            meshRenderer.materials = materials;
                        }
                    }
            }
        }
    }
}