using Components.Ui;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Systems.Ui.Initialize
{
    public class InGameUiInitializeSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<InGameMenuUiComponent> gameMenuPool = null;
        
        private readonly EcsWorldInject world = null;
        
        public void Init(IEcsSystems systems)
        {
            ref var gameMenuComponent = ref gameMenuPool.NewEntity(out var gameMenuEntity);
            gameMenuComponent.MenuBlock = UiHelper.InstantiateCanvasFromAsset("Prefabs/UI/GameMenu", world.Value);
        }
    }
}