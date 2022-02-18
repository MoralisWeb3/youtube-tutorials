using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.U2D.Animation
{
    internal class WeightInspectorIMGUIPanel : VisualElement
    {
        public class CustomUXMLFactor : UxmlFactory<WeightInspectorIMGUIPanel, UxmlTraits> {}
        private WeightInspector m_WeightInspector = new WeightInspector();

        public WeightInspector weightInspector
        {
            get { return m_WeightInspector; }
        }

        public event Action weightsChanged = () => {};

        public WeightInspectorIMGUIPanel()
        {
            name = "WeightInspectorIMGUIPanel";
            styleSheets.Add(ResourceLoader.Load<StyleSheet>("SkinningModule/WeightInspectorIMGUIPanelStyle.uss"));

            this.Add(new IMGUIContainer(OnGUI));
            this.pickingMode = PickingMode.Ignore;
            this.RegisterCallback<MouseDownEvent>((e) => { e.StopPropagation(); });
            this.RegisterCallback<MouseUpEvent>((e) => { e.StopPropagation(); });
        }

        protected void OnGUI()
        {
            var selectionCount = 0;

            if (weightInspector.selection != null)
                selectionCount = weightInspector.selection.Count;

            using (new EditorGUI.DisabledGroupScope(m_WeightInspector.spriteMeshData == null || selectionCount == 0))
            {
                GUILayout.Label(new GUIContent(TextContent.vertexWeight, TextContent.vertexWeightToolTip));
                EditorGUI.BeginChangeCheck();
                m_WeightInspector.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                    weightsChanged();
            }
        }
    }
}
