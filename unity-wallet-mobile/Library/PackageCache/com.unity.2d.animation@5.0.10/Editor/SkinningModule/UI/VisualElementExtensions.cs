using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal static class VisualElementExtensions
    {
        public static void SetHiddenFromLayout(this VisualElement element, bool isHidden)
        {
            if (isHidden)
            {
                element.SetEnabled(false);
                element.style.display = DisplayStyle.None;
                element.style.position = Position.Absolute;
            }
            else
            {
                element.SetEnabled(true);
                element.style.display = DisplayStyle.Flex;
                element.style.position = Position.Relative;
            }
        }
    }
}
