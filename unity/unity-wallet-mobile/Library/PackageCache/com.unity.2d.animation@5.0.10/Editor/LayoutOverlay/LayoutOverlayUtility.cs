using System;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Layout
{
    internal static class LayoutOverlayUtility
    {
        public static Button CreateButton(string name, Action clickEvent, string tooltip = null, string text = null, string imageResourcePath = null, string stylesheetPath = null)
        {
            Button button = new Button(clickEvent);
            button.name = name;
            button.tooltip = tooltip;

            if (!String.IsNullOrEmpty(text))
                button.text = text;
            if (!String.IsNullOrEmpty(imageResourcePath))
            {
                var texture = ResourceLoader.Load<Texture>(imageResourcePath);
                if (texture != null)
                {
                    Image image = new Image();
                    image.image = texture;
                    button.Add(image);
                }
            }
            if (!String.IsNullOrEmpty(stylesheetPath))
                button.styleSheets.Add(ResourceLoader.Load<StyleSheet>(stylesheetPath));

            return button;
        }
    }

}
