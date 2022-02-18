using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor
{
    static class SliderToggleUiHelper
    {
        static Texture2D s_ToggleOffTexture;
        static Texture2D s_ToggleOnTexture;

        static Texture2D ToggleOffTexture
        {
            get
            {
                if (s_ToggleOffTexture == null)
                {
                    s_ToggleOffTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(ImagePath.toggleOffPath);
                }
                return s_ToggleOffTexture;
            }
        }

        static Texture2D ToggleOnTexture
        {
            get
            {
                if (s_ToggleOnTexture == null)
                {
                    s_ToggleOnTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(ImagePath.toggleOnPath);
                }
                return s_ToggleOnTexture;
            }
        }

        public static void ConvertToggleToSliderToggle(Toggle toggle)
        {
            var label = toggle.Q<Label>();
            if (label != null)
            {
                label.text = null;
                label.visible = false;
            }
            toggle.AddToClassList(UssClassName.sliderToggle);
            toggle.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath.sliderToggleCommon));
            SetToggleImageBasedOnValue(toggle);
            toggle.RegisterValueChangedCallback(evt => {
                SetToggleImageBasedOnValue(toggle);
            });
        }

        static void SetToggleImageBasedOnValue(Toggle toggle)
        {
            var toggleImage = toggle.Q<VisualElement>(UxmlNodeName.unityCheckmark);
            if (toggleImage != null)
            {
                toggleImage.style.backgroundImage = toggle.value ?
                    new StyleBackground(ToggleOnTexture) :
                    new StyleBackground(ToggleOffTexture);
            }
        }

        static class ImagePath
        {
            internal const string toggleOffPath = "Packages/com.unity.services.core/Editor/Core/UiHelpers/Images/ToggleOff.png";
            internal const string toggleOnPath = "Packages/com.unity.services.core/Editor/Core/UiHelpers/Images/ToggleOn.png";
        }

        static class UxmlNodeName
        {
            internal const string unityCheckmark = "unity-checkmark";
        }

        static class UssClassName
        {
            internal const string sliderToggle = "slider-toggle";
        }

        static class UssPath
        {
            internal const string sliderToggleCommon = "Packages/com.unity.services.core/Editor/Core/UiHelpers/USS/SliderToggleCommon.uss";
        }
    }
}
