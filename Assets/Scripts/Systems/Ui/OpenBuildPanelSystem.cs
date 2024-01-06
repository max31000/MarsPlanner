using Components;
using Components.Input;
using Components.Ui;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Systems.Ui
{
    public class OpenBuildPanelSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ButtonComponent, ButtonOnClickEvent, ButtonReadyToProcessComponent>> buttonEvents = null;
        private readonly EcsPoolInject<ButtonComponent> buttonPool = null;
        private readonly EcsFilterInject<Inc<InGameMenuUiComponent>> inGameMenuFilter = null;
        private readonly EcsPoolInject<InGameMenuUiComponent> inGameMenuPool = null;
        private readonly EcsPoolInject<ButtonOnClickEvent> onClickEventPool = null;

        private readonly EcsFilterInject<Inc<RaycastObjectEvent>> raycastEventFilter = null;

        public void Run(IEcsSystems systems)
        {
            ProcessClosePanel();
            ProcessOpenPanel();
        }

        private void ProcessClosePanel()
        {
            if (raycastEventFilter.Value.GetEntitiesCount() == 0)
                return;
            
            SetActive(false);
        }

        private void ProcessOpenPanel()
        {
            if (buttonEvents.Value.GetEntitiesCount() == 0)
                return;

            foreach (var buttonEventEntity in buttonEvents.Value)
                ProcessButtonEvent(buttonEventEntity);
        }

        private void ProcessButtonEvent(int buttonEventEntity)
        {
            ref var buttonComponent = ref buttonPool.Value.Get(buttonEventEntity);

            if (buttonComponent.ButtonName != "OpenBuildPanel")
                return;

            SetActive(true);
        }

        private void SetActive(bool isActive)
        {
            var inGameMenuEntity = inGameMenuFilter.Value.Single();
            ref var inGameMenuComponent = ref inGameMenuPool.Value.Get(inGameMenuEntity);

            if (inGameMenuComponent.BuildPanel.activeSelf == isActive)
                return;

            inGameMenuComponent.BuildPanel.SetActive(isActive);
        }
    }
}