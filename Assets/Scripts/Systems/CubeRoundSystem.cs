using Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Systems
{
    public class CubeRoundSystem : IEcsRunSystem
    {
        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();

            var filter = world.Filter<CubeComponent>().End();

            var cubePool = world.GetPool<CubeComponent>();

            foreach (var element in filter)
            {
                var cubeComponent = cubePool.Get(element);
                var deltaTime = Time.deltaTime;
                
                cubeComponent.Cube.transform.Rotate(0, 100 * deltaTime, 0);
            }
        }
    }
}