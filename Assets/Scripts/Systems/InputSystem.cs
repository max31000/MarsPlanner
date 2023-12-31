using System;
using System.Collections.Generic;
using Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    internal class InputSystem : IEcsRunSystem
    {
        private readonly EcsPoolInject<PressedKeysKeeperComponent> keyKeeperPool = null;
        private readonly EcsFilterInject<Inc<PressedKeysKeeperComponent>> keysKeeperFilter = null;
        private readonly EcsPoolInject<PressKeyEvent> pressKeyEventsPool = null;

        private readonly EcsWorldInject world = null;

        public void Run(IEcsSystems systems)
        {
            if (keysKeeperFilter.Value.GetEntitiesCount() == 0)
            {
                var pressedKeyEntity = world.Value.NewEntity();
                ref var keysKeeperComponent = ref keyKeeperPool.Value.Add(pressedKeyEntity);
                keysKeeperComponent.pressedKeyCodeEvents = new Dictionary<KeyCode, int>();
            }

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

                if (Input.GetKeyUp(key))
                    if (keysKeeper.pressedKeyCodeEvents.ContainsKey(key))
                    {
                        pressKeyEventsPool.Value.Del(keysKeeper.pressedKeyCodeEvents[key]);
                        keysKeeper.pressedKeyCodeEvents.Remove(key);
                    }

                if (Input.GetKeyDown(key))
                {
                    if (keysKeeper.pressedKeyCodeEvents.ContainsKey(key))
                    {
                        pressKeyEventsPool.Value.Del(keysKeeper.pressedKeyCodeEvents[key]);
                        keysKeeper.pressedKeyCodeEvents.Remove(key);
                    }

                    var pressEventEntity = world.Value.NewEntity();
                    ref var pressEventComponent = ref pressKeyEventsPool.Value.Add(pressEventEntity);
                    pressEventComponent.Code = key;

                    keysKeeper.pressedKeyCodeEvents.Add(key, pressEventEntity);
                }
            }
        }
    }
}