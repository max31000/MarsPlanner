using Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class CubeSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<CubeComponent>> cubeFilter = null;
        private readonly EcsPoolInject<CubeComponent> cubePool = null;
        private readonly EcsFilterInject<Inc<RaycastObjectEvent>> raycastEventFilter = null;
        private readonly EcsPoolInject<RaycastObjectEvent> raycastEventPool = null;

        private readonly EcsWorldInject world = null;

        private int cubeRotationDirection = 1;

        public void Run(IEcsSystems systems)
        {
            foreach (var raycastEventEntity in raycastEventFilter.Value)
            {
                ref var rayCastEventComponent = ref raycastEventPool.Value.Get(raycastEventEntity);
                if (rayCastEventComponent.GameObjectName == "Cube(Clone)")
                    cubeRotationDirection *= -1;
            }
            foreach (var element in cubeFilter.Value)
            {
                ref var cubeComponent = ref cubePool.Value.Get(element);
                var deltaTime = Time.deltaTime;

                cubeComponent.Cube.transform.Rotate(0, cubeRotationDirection * 100 * deltaTime, 0);
            }
        }
    }
}