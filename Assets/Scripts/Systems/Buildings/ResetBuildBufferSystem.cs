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
        private readonly EcsFilterInject<Inc<ResetBufferEvent>> resetBufferEventFilter = null;
        private readonly EcsPoolInject<ResetBufferEvent> resetBufferEventPool = null;


        public void Run(IEcsSystems systems)
        {
            if (resetBufferEventFilter.Value.GetEntitiesCount() == 0)
                return;

            ref var bufferComponent = ref buildingBufferPool.Value.Get(buildingBufferFilter.Value.Single());

            foreach (var resetBufferEntity in resetBufferEventFilter.Value)
            {
                ref var resetBufferEvent = ref resetBufferEventPool.Value.Get(resetBufferEntity);
                var objectToReset = bufferComponent.BuildingsBuffer[resetBufferEvent.Type].InstancedBuilding;
                objectToReset.transform.position = BufferConstants.BufferObjectsPosition;
                resetBufferEventPool.Value.Del(resetBufferEntity);
            }
        }
    }
}