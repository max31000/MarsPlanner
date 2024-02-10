using Components.Buildings;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Systems.Buildings
{
    public class BuildingExitEventDeleteSystem : IEcsPostRunSystem
    {
        private readonly EcsFilterInject<Inc<BuildingModeExitEvent>> buildingModeExitEventFilter = null;
        
        public void PostRun(IEcsSystems systems)
        {
            foreach (var buildingEvent in buildingModeExitEventFilter.Value)
            {
                ref var buildingModeExitEvent = ref buildingModeExitEventFilter.Pools.Inc1.Get(buildingEvent);
                if (buildingModeExitEvent.IsNew)
                {
                    buildingModeExitEvent.IsNew = false;
                }
                else
                {
                    buildingModeExitEventFilter.Pools.Inc1.Del(buildingEvent);
                }
            }
        }
    }
}