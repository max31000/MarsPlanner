using System;
using Components.Buildings;
using Components.Input;
using Components.Ui;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Buildings;
using UnityEngine;

namespace Systems.Buildings
{
    public class StartBuildingSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<BuildingsBufferComponent>> buildingBufferFilter = null;
        private readonly EcsPoolInject<BuildingsBufferComponent> buildingBufferPool = null;

        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> buildPlaceFilter = null;
        private readonly EcsPoolInject<PlaceBuildProcessingComponent> buildPlacePool = null;

        private readonly EcsPoolInject<ButtonComponent> buttonPool = null;

        private readonly EcsFilterInject<Inc<CoordinatesCollectorComponent>> coordinatesCollectorFilter = null;
        private readonly EcsPoolInject<CoordinatesCollectorComponent> coordinatesCollectorPool = null;
        private readonly EcsFilterInject<Inc<ButtonOnClickEvent, ButtonReadyToProcessComponent>> onClickFilter = null;

        private readonly EcsFilterInject<Inc<RaycastTargetComponent>> raycastTargetFilter = null;

        private readonly EcsPoolInject<ResetBufferEvent> resetBufferPool = null;


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

            if (coordinatesCollectorFilter.Value.GetEntitiesCount() == 0)
                coordinatesCollectorPool.Value.Add(raycastTarget);
        }

        private void CreateBuildPlaceComponent(object resultType)
        {
            var buildType = (BuildingTypes)resultType;
            if (buildPlaceFilter.Value.GetEntitiesCount() == 0)
            {
                ref var buildPlaceComponent = ref buildPlacePool.NewEntity(out var _);
                UpdateBuildComponentData(buildType, ref buildPlaceComponent, true);
            }

            foreach (var buildPlaceEntity in buildPlaceFilter.Value)
            {
                ref var buildPlaceComponent = ref buildPlacePool.Value.Get(buildPlaceEntity);
                UpdateBuildComponentData(buildType, ref buildPlaceComponent, false);
            }
        }

        private void UpdateBuildComponentData(BuildingTypes buildType,
            ref PlaceBuildProcessingComponent buildPlaceComponent, bool newBuilding)
        {
            ref var buildBuffer = ref buildingBufferPool.Value.Get(buildingBufferFilter.Value.Single());
            var buildingBufferObject = buildBuffer.BuildingsBuffer[buildPlaceComponent.Type];

            if (buildType != buildPlaceComponent.Type && !newBuilding)
            {
                ref var resetBufferEvent = ref resetBufferPool.NewEntity(out _);
                resetBufferEvent.Type = buildPlaceComponent.Type;
            }

            buildPlaceComponent.Size =
                buildingBufferObject.InstancedBuilding.GetComponentInChildren<Collider>().bounds.size;
            buildPlaceComponent.Type = buildType;
        }
    }
}