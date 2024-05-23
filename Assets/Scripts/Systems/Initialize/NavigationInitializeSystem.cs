using Components.Navigation;
using Components.World;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Models.Navigation;

namespace Systems.Initialize
{
    public class NavigationInitializeSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<GlobalNavigationDataComponent> globalNavigationDataPool = null;
        private readonly EcsFilterInject<Inc<WorldTerrainKeeperComponent>> worldTerrainFilter = null;

        public void Init(IEcsSystems systems)
        {
            foreach (var terrainEntity in worldTerrainFilter.Value)
            {
                ref var terrainComponent = ref worldTerrainFilter.Pools.Inc1.Get(terrainEntity);

                var graph = new NavigationSystem(terrainComponent.Terrain, 3, 10, 30);

                ref var navigationComponent = ref globalNavigationDataPool.NewEntity(out var _);
                navigationComponent.NavigationSystem = graph;

                graph.DrawGraph();
            }
        }
    }
}