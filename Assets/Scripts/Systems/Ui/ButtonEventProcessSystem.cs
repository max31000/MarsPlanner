using Components.Ui;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Systems.Ui
{
    public class ButtonEventProcessSystem : IEcsPostRunSystem
    {
        private readonly EcsPoolInject<ButtonOnClickEvent> buttonEventPool = null;
        private readonly EcsFilterInject<Inc<ButtonComponent, ButtonOnClickEvent>> buttonFilter = null;
        private readonly EcsPoolInject<ButtonReadyToProcessComponent> buttonReadyToProcessPool = null;

        private readonly EcsWorldInject world = null;

        public void PostRun(IEcsSystems systems)
        {
            if (buttonFilter.Value.GetEntitiesCount() == 0)
                return;

            foreach (var buttonEntity in buttonFilter.Value) ProcessEntity(buttonEntity);
        }

        private void ProcessEntity(int buttonEntity)
        {
            if (!buttonReadyToProcessPool.Value.Has(buttonEntity))
            {
                buttonReadyToProcessPool.Value.Add(buttonEntity);
            }
            else
            {
                buttonEventPool.Value.Del(buttonEntity);
                buttonReadyToProcessPool.Value.Del(buttonEntity);
            }
        }
    }
}