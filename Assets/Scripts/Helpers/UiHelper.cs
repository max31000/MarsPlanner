using Components.Ui;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers
{
    public class UiHelper
    {
        public static Canvas InstantiateCanvasFromAsset(string assetPath, EcsWorld world)
        {
            var canvasAsset = Resources.Load<Canvas>(assetPath);
            return InstantiateAllButtons(canvasAsset, world);
        }

        public static Canvas InstantiateAllButtons(Canvas canvasAsset, EcsWorld world)
        {
            var newCanvas = Object.Instantiate(canvasAsset);

            foreach (var button in newCanvas.GetComponentsInChildren<Button>())
            {
                var buttonEntity = world.NewEntity();
                ref var buttonComponent = ref world.GetPool<ButtonComponent>().Add(buttonEntity);
                buttonComponent.ButtonName = button.name;
                buttonComponent.Bounds = button.GetButtonBounds();
                buttonComponent.Button = button;
                //buttonComponent.Button.onClick.AddListener(() => OnClick(buttonEntity));
            }

            return newCanvas;
        }
    }
}