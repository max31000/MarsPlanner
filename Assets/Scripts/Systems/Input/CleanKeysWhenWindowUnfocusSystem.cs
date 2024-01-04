using Components;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class CleanKeysWhenWindowUnfocusSystem : IEcsRunSystem
    {
        private readonly EcsPoolInject<PressedKeysKeeperComponent> keyKeeperPool = null;
        private readonly EcsFilterInject<Inc<PressedKeysKeeperComponent>> keysKeeperFilter = null;
        private readonly EcsFilterInject<Inc<InputKeyPressedEvent>> pressKeyEventFilter = null;
        private readonly EcsPoolInject<InputKeyPressedEvent> pressKeyEventsPool = null;
        private readonly EcsFilterInject<Inc<PreviousWindowFocusStateComponent>> windowStateFilter = null;
        private readonly EcsPoolInject<PreviousWindowFocusStateComponent> windowStatePool = null;

        private readonly EcsWorldInject world = null;

        public void Run(IEcsSystems systems)
        {
            if (windowStateFilter.Value.GetEntitiesCount() == 0)
            {
                var windowStateNewEntity = world.Value.NewEntity();
                windowStatePool.Value.Add(windowStateNewEntity);
            }

            var windowStateEntity = windowStateFilter.Value.Single();
            ref var windowStateComponent = ref windowStatePool.Value.Get(windowStateEntity);

            var isFocused = Application.isFocused;
            if (windowStateComponent.IsFocused != isFocused)
            {
                CleanAllKeys();
            }

            windowStateComponent.IsFocused = isFocused;
        }

        private void CleanAllKeys()
        {
            foreach (var keysKeeper in keysKeeperFilter.Value)
            {
                ref var keysKeeperComponent = ref keyKeeperPool.Value.Get(keysKeeper);
                keysKeeperComponent.PressedKeyCodeEvents.Clear();
            }

            foreach (var pressedKeyEvents in pressKeyEventFilter.Value)
            {
                pressKeyEventsPool.Value.Del(pressedKeyEvents);
            }
        }
    }
}