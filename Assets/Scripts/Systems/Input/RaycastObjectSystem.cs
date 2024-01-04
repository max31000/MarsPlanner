﻿using Components;
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
            HandleMouseUp();
        }

        private void RemoveOldEvents()
        {
            castObjectPool.Value.Del(castObjectFilter.Value.Single());
        }

        private void HandleMouseUp()
        {
            if (keyUpInputFilter.Value.GetEntitiesCount() == 0) return;

            foreach (var keyUpEntity in keyUpInputFilter.Value)
            {
                ref var keyComponent = ref keyInputPool.Value.Get(keyUpEntity);
                if (keyComponent.Code == KeyCode.Mouse0)
                    HandleRaycast();
            }
        }

        private void HandleRaycast()
        {
            var cameraEntity = cameraComponentsFilter.Value.Single();
            ref var cameraComponent = ref cameraComponentsPool.Value.Get(cameraEntity);
            var ray = cameraComponent.Camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, 300))
                return;

            ref var castObjectComponent = ref castObjectPool.Value.Add(castObjectFilter.Value.Single());
            castObjectComponent.GameObject = hit.collider.gameObject;
            castObjectComponent.GameObjectName = castObjectComponent.GameObject.name;
        }
    }
}