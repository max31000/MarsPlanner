using System.Linq;
using Components.Ui;
using Helpers;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

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
            gameMenuComponent.BuildPanel = CreateBuildPanel(gameMenuComponent);
        }

        private static GameObject CreateBuildPanel(InGameMenuUiComponent gameMenuComponent)
        {
            var allRectTransformComponents = gameMenuComponent.MenuBlock.GetComponentsInChildren<RectTransform>();
            var buildPanel = allRectTransformComponents.Single(x => x.gameObject.name == "BuildPanel");
            var gameObject = buildPanel.gameObject;
            gameObject.SetActive(false);
            return gameObject;
        }
    }
}