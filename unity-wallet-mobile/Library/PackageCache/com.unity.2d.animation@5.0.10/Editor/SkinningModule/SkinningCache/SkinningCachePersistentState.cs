using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal interface ISkinningCachePersistentState
    {
        String lastSpriteId
        {
            get;
            set;
        }

        Tools lastUsedTool
        {
            get;
            set;
        }

        List<int> lastBoneSelectionIds
        {
            get;
        }

        Texture2D lastTexture
        {
            get;
            set;
        }

        SerializableDictionary<int, BonePose> lastPreviewPose
        {
            get;
        }

        SerializableDictionary<int, bool> lastBoneVisibility
        {
            get;
        }

        SerializableDictionary<int, bool> lastBoneExpansion
        {
            get;
        }

        SerializableDictionary<string, bool> lastSpriteVisibility
        {
            get;
        }

        SerializableDictionary<int, bool> lastGroupVisibility
        {
            get;
        }

        SkinningMode lastMode
        {
            get;
            set;
        }

        bool lastVisibilityToolActive
        {
            get;
            set;
        }

        int lastVisibilityToolIndex
        {
            get;
            set;
        }

        IndexedSelection lastVertexSelection
        {
            get;
        }

        float lastBrushSize
        {
            get;
            set;
        }

        float lastBrushHardness
        {
            get;
            set;
        }

        float lastBrushStep
        {
            get;
            set;
        }
    }

    [Serializable]
    internal class SkinningCachePersistentState
        : ScriptableSingleton<SkinningCachePersistentState>
        , ISkinningCachePersistentState
    {
        [SerializeField] private Tools m_LastUsedTool = Tools.EditPose;

        [SerializeField] private SkinningMode m_LastMode = SkinningMode.Character;

        [SerializeField] private string m_LastSpriteId = String.Empty;

        [SerializeField] private List<int> m_LastBoneSelectionIds = new List<int>();

        [SerializeField] private Texture2D m_LastTexture;

        [SerializeField]
        private SerializableDictionary<int, BonePose> m_SkeletonPreviewPose =
            new SerializableDictionary<int, BonePose>();

        [SerializeField]
        private SerializableDictionary<int, bool> m_BoneVisibility =
            new SerializableDictionary<int, bool>();

        [SerializeField]
        private SerializableDictionary<int, bool> m_BoneExpansion =
            new SerializableDictionary<int, bool>();

        [SerializeField]
        private SerializableDictionary<string, bool> m_SpriteVisibility =
            new SerializableDictionary<string, bool>();

        [SerializeField]
        private SerializableDictionary<int, bool> m_GroupVisibility =
            new SerializableDictionary<int, bool>();

        [SerializeField] private IndexedSelection m_VertexSelection;

        [SerializeField] private bool m_VisibilityToolActive;
        [SerializeField] private int m_VisibilityToolIndex = -1;

        [SerializeField] private float m_LastBrushSize = 25f;
        [SerializeField] private float m_LastBrushHardness = 1f;
        [SerializeField] private float m_LastBrushStep = 20f;

        public SkinningCachePersistentState()
        {
            m_VertexSelection = new IndexedSelection();
        }

        public void SetDefault()
        {
            m_LastUsedTool = Tools.EditPose;
            m_LastMode = SkinningMode.Character;
            m_LastSpriteId = String.Empty;
            m_LastBoneSelectionIds.Clear();
            m_LastTexture = null;
            m_VertexSelection.Clear();
            m_SkeletonPreviewPose.Clear();
            m_BoneVisibility.Clear();
            m_BoneExpansion.Clear();
            m_SpriteVisibility.Clear();
            m_GroupVisibility.Clear();
            m_VisibilityToolActive = false;
            m_VisibilityToolIndex = -1;
        }

        public string lastSpriteId
        {
            get { return m_LastSpriteId; }
            set { m_LastSpriteId = value; }
        }

        public Tools lastUsedTool
        {
            get { return m_LastUsedTool; }
            set { m_LastUsedTool = value; }
        }

        public List<int> lastBoneSelectionIds
        {
            get { return m_LastBoneSelectionIds; }
        }

        public Texture2D lastTexture
        {
            get { return m_LastTexture; }
            set
            {
                if (value != m_LastTexture)
                {
                    m_LastMode = SkinningMode.Character;
                    m_LastSpriteId = String.Empty;
                    m_LastBoneSelectionIds.Clear();
                    m_VertexSelection.Clear();
                    m_SkeletonPreviewPose.Clear();
                    m_BoneVisibility.Clear();
                    m_BoneExpansion.Clear();
                    m_SpriteVisibility.Clear();
                    m_GroupVisibility.Clear();
                }

                m_LastTexture = value;
            }
        }

        public SerializableDictionary<int, BonePose> lastPreviewPose
        {
            get { return m_SkeletonPreviewPose; }
        }

        public SerializableDictionary<int, bool> lastBoneVisibility
        {
            get { return m_BoneVisibility; }
        }

        public SerializableDictionary<int, bool> lastBoneExpansion
        {
            get { return m_BoneExpansion; }
        }

        public SerializableDictionary<string, bool> lastSpriteVisibility
        {
            get { return m_SpriteVisibility; }
        }

        public SerializableDictionary<int, bool> lastGroupVisibility
        {
            get { return m_GroupVisibility; }
        }

        public SkinningMode lastMode
        {
            get { return m_LastMode; }
            set { m_LastMode = value; }
        }

        public bool lastVisibilityToolActive
        {
            get { return m_VisibilityToolActive; }
            set { m_VisibilityToolActive = value; }
        }

        public int lastVisibilityToolIndex
        {
            get { return m_VisibilityToolIndex; }
            set { m_VisibilityToolIndex = value; }
        }

        public IndexedSelection lastVertexSelection
        {
            get { return m_VertexSelection; }
        }

        public float lastBrushSize
        {
            get { return m_LastBrushSize; }
            set { m_LastBrushSize = value; }
        }

        public float lastBrushHardness
        {
            get { return m_LastBrushHardness; }
            set { m_LastBrushHardness = value; }
        }

        public float lastBrushStep
        {
            get { return m_LastBrushStep; } 
            set { m_LastBrushStep = value; }
        }
    }
}