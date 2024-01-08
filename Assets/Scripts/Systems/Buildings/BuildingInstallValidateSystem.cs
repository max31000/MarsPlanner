using Components.Buildings;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Systems.Buildings
{
    public class BuildingInstallValidateSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> buildPlaceFilter = null;
        private readonly EcsPoolInject<PlaceBuildProcessingComponent> buildPlacePool = null;
        
        public void Run(IEcsSystems systems)
        {
            if(buildPlaceFilter.Value.GetEntitiesCount() == 0)
                return;

            ref var buildingPlaceComponent = ref buildPlacePool.Value.Get(buildPlaceFilter.Value.Single());
            buildingPlaceComponent.IsCantInstall = true;
        }
    }
}