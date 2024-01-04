using System.Linq;
using Components;
using Definitions;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class RaycastObjectSystem : IEcsPostRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<CameraComponent>> cameraComponentsFilter = null;
        private readonly EcsPoolInject<CameraComponent> cameraComponentsPool = null;
        private readonly EcsPoolInject<RaycastObjectEntityComponent> castObjectEntityPool = null;
        private readonly EcsFilterInject<Inc<RaycastObjectEntityComponent>> castObjectFilter = null;
        private readonly EcsPoolInject<RaycastObjectEvent> castObjectPool = null;
        private readonly EcsPoolInject<InputKeyPressedEvent> keyInputPool = null;
        private readonly EcsFilterInject<Inc<InputKeyPressedEvent, InputKeyUpEvent>> keyUpInputFilter = null;
        
        private readonly EcsCustomInject<GameDefinitions> definitions = default;

        private readonly EcsWorldInject world = null;

        public void Init(IEcsSystems systems)
        {
            if (castObjectFilter.Value.GetEntitiesCount() != 0)
                return;

            var raycastEntity = world.Value.NewEntity();
            castObjectEntityPool.Value.Add(raycastEntity);
        }

        public void PostRun(IEcsSystems systems)
        {
            RemoveOldEvents();
            HandleKeysUp();
        }

        private void RemoveOldEvents()
        {
            castObjectPool.Value.Del(castObjectFilter.Value.Single());
        }

        private void HandleKeysUp()
        {
            if (keyUpInputFilter.Value.GetEntitiesCount() == 0) return;

            foreach (var keyUpEntity in keyUpInputFilter.Value)
            {
                ref var keyComponent = ref keyInputPool.Value.Get(keyUpEntity);
                if (definitions.Value.KeysDefinitions.KeysWithRaycastObserving.Contains(keyComponent.Code))
                    HandleRaycast(keyComponent.Code);
            }
        }

        private void HandleRaycast(KeyCode sourceKeyCode)
        {
            var cameraEntity = cameraComponentsFilter.Value.Single();
            ref var cameraComponent = ref cameraComponentsPool.Value.Get(cameraEntity);
            var ray = cameraComponent.Camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, 300))
                return;

            ref var castObjectComponent = ref castObjectPool.Value.Add(castObjectFilter.Value.Single());
            castObjectComponent.GameObject = hit.collider.gameObject;
            castObjectComponent.GameObjectName = castObjectComponent.GameObject.name;
            castObjectComponent.RaySourceKeyCode = sourceKeyCode;
        }
    }
}