using System;
using System.Collections.Generic;
using Components;
using Components.Input;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems.Input
{
    internal class InputSystem : IEcsRunSystem, IEcsPostRunSystem
    {
        private readonly EcsPoolInject<PressedKeysKeeperComponent> keyKeeperPool = null;
        private readonly EcsFilterInject<Inc<PressedKeysKeeperComponent>> keysKeeperFilter = null;
        private readonly EcsPoolInject<InputKeyPressedEvent> pressKeyEventsPool = null;
        private readonly EcsPoolInject<InputKeyUpEvent> pressKeyUpEventsPool = null;
        private readonly EcsPoolInject<InputKeyDownEvent> pressKeyDownEventsPool = null;
        private readonly EcsFilterInject<Inc<InputKeyUpEvent>> pressKeyUpEventsFilter = null;

        private readonly EcsWorldInject world = null;

        public void Run(IEcsSystems systems)
        {
            if (keysKeeperFilter.Value.GetEntitiesCount() == 0)
            {
                var pressedKeyEntity = world.Value.NewEntity();
                ref var keysKeeperComponent = ref keyKeeperPool.Value.Add(pressedKeyEntity);
                keysKeeperComponent.PressedKeyCodeEvents = new Dictionary<KeyCode, int>();
            }
        }

        public void PostRun(IEcsSystems systems)
        {
            foreach (var keysKeeperId in keysKeeperFilter.Value)
            {
                ref var keysKeeperComponent = ref keyKeeperPool.Value.Get(keysKeeperId);
                KeyboardHandle(ref keysKeeperComponent);
            }
        }

        private void KeyboardHandle(ref PressedKeysKeeperComponent keysKeeper)
        {
            foreach (var e in Enum.GetValues(typeof(KeyCode)))
            {
                var key = (KeyCode)e;

                HandleKeyDownEvent(ref keysKeeper, key);
                HandleKeyBetweenDownAndUp(ref keysKeeper, key);
                HandleUpKeyEvent(ref keysKeeper, key);
            }
        }

        private void HandleKeyBetweenDownAndUp(ref PressedKeysKeeperComponent keysKeeper, KeyCode key)
        {
            if (UnityEngine.Input.GetKey(key) && !UnityEngine.Input.GetKeyUp(key) && !UnityEngine.Input.GetKeyDown(key)
                && keysKeeper.PressedKeyCodeEvents.TryGetValue(key, out var pressEventEntity))
            {
                ref var pressEventComponent = ref pressKeyEventsPool.Value.Get(pressEventEntity);
                pressKeyDownEventsPool.Value.Del(pressEventEntity);
                pressKeyUpEventsPool.Value.Del(pressEventEntity);
            }
        }

        private void HandleKeyDownEvent(ref PressedKeysKeeperComponent keysKeeper, KeyCode key)
        {
            if (!UnityEngine.Input.GetKeyDown(key))
                return;

            if (keysKeeper.PressedKeyCodeEvents.ContainsKey(key))
            {
                pressKeyEventsPool.Value.Del(keysKeeper.PressedKeyCodeEvents[key]);
                keysKeeper.PressedKeyCodeEvents.Remove(key);
            }

            var pressEventEntity = world.Value.NewEntity();
            ref var pressEventComponent = ref pressKeyEventsPool.Value.Add(pressEventEntity);
            pressEventComponent.Code = key;
            pressKeyDownEventsPool.Value.Add(pressEventEntity);

            keysKeeper.PressedKeyCodeEvents.Add(key, pressEventEntity);
        }

        private void HandleUpKeyEvent(ref PressedKeysKeeperComponent keysKeeper, KeyCode key)
        {
            if (!keysKeeper.PressedKeyCodeEvents.TryGetValue(key, out var pressedKeyCodeEntity))
                return;

            ref var component = ref pressKeyEventsPool.Value.Get(pressedKeyCodeEntity);

            if (pressKeyUpEventsFilter.Value.Contains(pressedKeyCodeEntity))
            {
                pressKeyEventsPool.Value.Del(pressedKeyCodeEntity);
                pressKeyUpEventsPool.Value.Del(pressedKeyCodeEntity);
                keysKeeper.PressedKeyCodeEvents.Remove(key);
            }

            if (UnityEngine.Input.GetKeyUp(key))
            {
                pressKeyUpEventsPool.Value.Add(pressedKeyCodeEntity);
            }
        }
    }
}