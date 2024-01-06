using System;
using Components.Buildings;
using Components.Input;
using Components.Ui;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Buildings;

namespace Systems.Buildings
{
    public class StartBuildingSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> buildPlaceFilter = null;
        private readonly EcsPoolInject<PlaceBuildProcessingComponent> buildPlacePool = null;

        private readonly EcsPoolInject<ButtonComponent> buttonPool = null;
        private readonly EcsFilterInject<Inc<ButtonOnClickEvent, ButtonReadyToProcessComponent>> onClickFilter = null;

        private readonly EcsFilterInject<Inc<CoordinatesCollectorComponent>> coordinatesCollectorFilter = null;
        private readonly EcsPoolInject<CoordinatesCollectorComponent> coordinatesCollectorPool = null;

        private readonly EcsFilterInject<Inc<RaycastTargetComponent>> raycastTargetFilter = null;
        private readonly EcsPoolInject<RaycastTargetComponent> raycastTargetPool = null;


        public void Run(IEcsSystems systems)
        {
            if (onClickFilter.Value.GetEntitiesCount() == 0)
                return;

            foreach (var clickEvent in onClickFilter.Value)
                ProcessOnClickEvent(clickEvent);
        }

        private void ProcessOnClickEvent(int clickEvent)
        {
            ref var buttonComponent = ref buttonPool.Value.Get(clickEvent);

            if (!Enum.TryParse(typeof(BuildingTypes), buttonComponent.ButtonName, out var resultType))
                return;

            CreateRaycastCollector();
            CreateBuildPlaceComponent(resultType);
        }

        private void CreateRaycastCollector()
        {
            var raycastTarget = raycastTargetFilter.Value.Single();

            foreach (var coordinatesCollectorEntity in coordinatesCollectorFilter.Value)
                coordinatesCollectorPool.Value.Del(coordinatesCollectorEntity);

            coordinatesCollectorPool.Value.Add(raycastTarget);
        }

        private void CreateBuildPlaceComponent(object resultType)
        {
            foreach (var buildPlaceEntity in buildPlaceFilter.Value)
                buildPlacePool.Value.Del(buildPlaceEntity);

            var buildType = (BuildingTypes)resultType;
            ref var buildPlaceComponent = ref buildPlacePool.NewEntity(out var _);
            buildPlaceComponent.Type = buildType;
        }
    }
}