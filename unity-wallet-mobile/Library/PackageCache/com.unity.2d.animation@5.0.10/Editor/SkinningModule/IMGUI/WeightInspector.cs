using UnityEngine;
using UnityEditor.U2D.Sprites;

namespace UnityEditor.U2D.Animation
{
    internal class WeightInspector
    {
        private SpriteMeshDataController m_SpriteMeshDataController = new SpriteMeshDataController();
        private GUIContent[] m_BoneNameContents;

        public ISpriteMeshData spriteMeshData
        {
            get { return m_SpriteMeshDataController.spriteMeshData; }
            set
            {
                if (spriteMeshData != value)
                    m_SpriteMeshDataController.spriteMeshData = value;
            }
        }

        public GUIContent[] boneNames
        {
            get { return m_BoneNameContents; }
            set { m_BoneNameContents = value; }
        }

        public ICacheUndo cacheUndo { get; set; }
        public ISelection<int> selection { get; set; }
        public int controlID { get { return 0; } }

        private bool m_UndoRegistered = false;

        protected ISpriteEditor spriteEditor
        {
            get; private set;
        }

        public void OnInspectorGUI()
        {
            ChannelsGUI();
        }

        private void ChannelsGUI()
        {
            if (GUIUtility.hotControl == 0)
                m_UndoRegistered = false;

            for (int channel = 0; channel < 4; ++channel)
            {
                var enabled = false;
                var boneIndex = -1;
                var weight = 0f;
                var isChannelEnabledMixed  = false;
                var isBoneIndexMixed  = false;
                var isWeightMixed  = false;
                
                if (spriteMeshData != null)
                    m_SpriteMeshDataController.GetMultiEditChannelData(selection, channel, out enabled, out boneIndex, out weight, out isChannelEnabledMixed, out isBoneIndexMixed, out isWeightMixed);

                var newEnabled = enabled;
                var newBoneIndex = boneIndex;
                var newWeight = weight;

                EditorGUI.BeginChangeCheck();

                WeightChannelDrawer(ref newEnabled, ref newBoneIndex, ref newWeight, isChannelEnabledMixed, isBoneIndexMixed, isWeightMixed);

                if (EditorGUI.EndChangeCheck())
                {
                    RegisterUndo();
                    m_SpriteMeshDataController.SetMultiEditChannelData(selection, channel, enabled, newEnabled, boneIndex, newBoneIndex, weight, newWeight);
                }
            }
        }

        private void WeightChannelDrawer(
            ref bool isChannelEnabled, ref int boneIndex, ref float weight,
            bool isChannelEnabledMixed = false, bool isBoneIndexMixed = false, bool isWeightMixed = false)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUIUtility.fieldWidth = 1f;
            EditorGUIUtility.labelWidth = 1f;

            EditorGUI.showMixedValue = isChannelEnabledMixed;
            isChannelEnabled = EditorGUILayout.Toggle(GUIContent.none, isChannelEnabled);

            EditorGUIUtility.fieldWidth = 30f;
            EditorGUIUtility.labelWidth = 30f;

            using (new EditorGUI.DisabledScope(!isChannelEnabled && !isChannelEnabledMixed))
            {
                int tempBoneIndex = GUI.enabled ? boneIndex : -1;

                EditorGUI.BeginChangeCheck();

                EditorGUIUtility.fieldWidth = 80f;

                EditorGUI.showMixedValue = GUI.enabled && isBoneIndexMixed;
                tempBoneIndex = EditorGUILayout.Popup(tempBoneIndex, m_BoneNameContents);

                if (EditorGUI.EndChangeCheck())
                    boneIndex = tempBoneIndex;

                EditorGUIUtility.fieldWidth = 32f;
                EditorGUI.showMixedValue = isWeightMixed;
                weight = EditorGUILayout.Slider(GUIContent.none, weight, 0f, 1f);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.showMixedValue = false;
            EditorGUIUtility.labelWidth = -1;
            EditorGUIUtility.fieldWidth = -1;
        }

        private void RegisterUndo()
        {
            if (m_UndoRegistered)
                return;

            Debug.Assert(cacheUndo != null);

            cacheUndo.BeginUndoOperation(TextContent.editWeights);

            m_UndoRegistered = true;
        }
    }
}
