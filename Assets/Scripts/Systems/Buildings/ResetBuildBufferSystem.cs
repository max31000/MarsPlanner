using Components.Buildings;
using Definitions.Constants;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Systems.Buildings
{
    public class ResetBuildBufferSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<BuildingsBufferComponent>> buildingBufferFilter = null;
        private readonly EcsPoolInject<BuildingsBufferComponent> buildingBufferPool = null;
        private readonly EcsFilterInject<Inc<BuildingModeExitEvent>> buildingModeExitEventFilter = null;
        private readonly EcsPoolInject<BuildingModeExitEvent> buildingModeExitEventPool = null;


        public void Run(IEcsSystems systems)
        {
            if (buildingModeExitEventFilter.Value.GetEntitiesCount() == 0)
                return;

            ref var bufferComponent = ref buildingBufferPool.Value.Get(buildingBufferFilter.Value.Single());

            foreach (var resetBufferEntity in buildingModeExitEventFilter.Value)
            {
                ref var resetBufferEvent = ref buildingModeExitEventPool.Value.Get(resetBufferEntity);
                var objectToReset = bufferComponent.BuildingsBuffer[resetBufferEvent.InstalledType].InstancedBuilding;
                objectToReset.transform.position = BufferConstants.BufferObjectsPosition;
                objectToReset.transform.rotation = BufferConstants.BufferObjectRotation;
            }
        }
    }
}