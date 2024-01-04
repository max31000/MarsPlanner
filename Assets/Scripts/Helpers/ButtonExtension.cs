using UnityEngine;
using UnityEngine.UI;

namespace Helpers
{
    public static class ButtonExtension
    {
        public static Bounds GetButtonBounds(this Button button)
        {
            return new Bounds(new Vector3(button.transform.position.x,
                    button.transform.position.y, 0),
                new Vector3(button.GetComponent<RectTransform>().rect.width,
                    button.GetComponent<RectTransform>().rect.height));
        }

        public static Bounds GetLayoutButtonBounds (this Button button)
        {
            return new Bounds(new Vector3(button.transform.localPosition.x,
                    button.transform.localPosition.y, 0),
                new Vector3(button.GetComponentInParent<GridLayoutGroup>().cellSize.x,
                    button.GetComponentInParent<GridLayoutGroup>().cellSize.y));
        }
    }
}