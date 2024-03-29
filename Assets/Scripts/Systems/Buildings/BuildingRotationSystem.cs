﻿using Components.Buildings;
using Components.Input;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Buildings;
using UnityEngine;

namespace Systems.Buildings
{
    public class BuildingRotationSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlaceBuildProcessingComponent>> placeBuildFilter = default;
        private readonly EcsFilterInject<Inc<InputKeyPressedEvent, InputKeyDownEvent>> pressKeyDownEventsFilter = null;

        public void Run(IEcsSystems systems)
        {
            if (placeBuildFilter.Value.GetEntitiesCount() == 0 ||
                pressKeyDownEventsFilter.Value.GetEntitiesCount() == 0)
                return;

            foreach (var pressDownEvent in pressKeyDownEventsFilter.Value)
            {
                ref var pressComponent = ref pressKeyDownEventsFilter.Pools.Inc1.Get(pressDownEvent);

                if (pressComponent.Code == KeyCode.R)
                    Rotate();
            }
        }

        private void Rotate()
        {
            foreach (var placeBuildEntity in placeBuildFilter.Value)
            {
                ref var placeBuildComponent = ref placeBuildFilter.Pools.Inc1.Get(placeBuildEntity);
                var rotateAngle = placeBuildComponent.Type is BuildingType.Passageway ? 45 : 90;

                placeBuildComponent.Rotation = new Vector3(
                    placeBuildComponent.Rotation.x,
                    placeBuildComponent.Rotation.y + rotateAngle,
                    placeBuildComponent.Rotation.z
                );
            }
        }
    }
}