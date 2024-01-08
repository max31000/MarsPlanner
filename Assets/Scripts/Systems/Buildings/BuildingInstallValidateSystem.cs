using System.Linq;
using Components.Buildings;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems.Buildings
{
    public class BuildingInstallValidateSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> buildPlaceFilter = null;
        private readonly EcsPoolInject<PlaceBuildProcessingComponent> buildPlacePool = null;

        private readonly EcsFilterInject<Inc<InstalledBuildComponent>> installedBuildFilter = null;
        private readonly EcsPoolInject<InstalledBuildComponent> installedBuildPool = null;

        public void Run(IEcsSystems systems)
        {
            foreach (var buildPlaceComponent in buildPlaceFilter.Value)
            {
                ref var buildingPlaceComponent = ref buildPlacePool.Value.Get(buildPlaceComponent);

                buildingPlaceComponent.IsCanInstall =
                    HasBuildIntersects(buildingPlaceComponent.Position, buildingPlaceComponent.Size);
            }
        }

        private bool HasBuildIntersects(Vector3 currentPos, Vector3 currentSize)
        {
            foreach (var installedBuildEntity in installedBuildFilter.Value)
            {
                ref var installedBuildComponent = ref installedBuildPool.Value.Get(installedBuildEntity);

                var canInstall = !installedBuildComponent
                    .Object.GetComponentsInChildren<Collider>()
                    .Any(b => b.bounds.Intersects(new Bounds(currentPos, currentSize)));

                if (!canInstall)
                    return false;
            }

            return true;
        }
    }
}