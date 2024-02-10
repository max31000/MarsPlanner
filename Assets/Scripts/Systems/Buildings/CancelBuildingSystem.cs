using Components.Buildings;
using Components.Input;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems.Buildings
{
    public class CancelBuildingSystem : IEcsPostRunSystem
    {
        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> buildPlaceFilter = null;

        private readonly EcsPoolInject<InputKeyPressedEvent> pressKeyPressEventsPool = null;
        private readonly EcsFilterInject<Inc<InputKeyPressedEvent, InputKeyUpEvent>> pressKeyUpEventsFilter = null;

        private readonly EcsPoolInject<BuildingModeExitEvent> buildingModeExitPool = null;


        public void PostRun(IEcsSystems systems)
        {
            if (buildPlaceFilter.Value.GetEntitiesCount() == 0 || pressKeyUpEventsFilter.Value.GetEntitiesCount() == 0)
                return;

            foreach (var pressUpEntity in pressKeyUpEventsFilter.Value)
            {
                ref var pressComponent = ref pressKeyPressEventsPool.Value.Get(pressUpEntity);

                if (pressComponent.Code is not (KeyCode.Escape or KeyCode.Mouse1))
                    continue;

                foreach (var buildPlaceEntity in buildPlaceFilter.Value)
                {
                    ref var buildPlaceComponent = ref buildPlaceFilter.Pools.Inc1.Get(buildPlaceEntity);
                    ref var buildingModeExitEvent = ref buildingModeExitPool.NewEntity(out _);
                    buildingModeExitEvent.InstalledType = buildPlaceComponent.Type;
                    buildingModeExitEvent.IsNew = true;
                    buildPlaceFilter.Pools.Inc1.Del(buildPlaceEntity);
                }
            }
        }
    }
}