using System;

namespace UnityEngine.Experimental.U2D.Animation
{
    /// <summary>
    /// Updates a SpriteRenderer's Sprite reference on the Category and Label value it is set
    /// </summary>
    /// <Description>
    /// By setting the SpriteResolver's Category and Label value, it will request for a Sprite from
    /// a SpriteLibrary Component the Sprite that is registered for the Category and Label.
    /// If a SpriteRenderer is present in the same GameObject, the SpriteResolver will update the
    /// SpriteRenderer's Sprite reference to the corresponding Sprite.
    /// </Description>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("2D Animation/Sprite Resolver (Experimental)")]
    [DefaultExecutionOrder(-2)]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.animation@latest/index.html?subfolder=/manual/SLAsset.html%23sprite-resolver-component")]
    public class SpriteResolver : MonoBehaviour
    {
        // These are for animation
        [SerializeField]
        private float m_CategoryHash;
        [SerializeField]
        private float m_labelHash;

        // For comparing hash values
        private int m_CategoryHashInt;
        private int m_LabelHashInt;

        // For OnUpdate during animation playback
        private int m_PreviousCategoryHash;
        private int m_PreviouslabelHash;

#if UNITY_EDITOR
        bool m_SpriteLibChanged;
#endif

        void OnEnable()
        {
            m_CategoryHashInt = ConvertFloatToInt(m_CategoryHash);
            m_PreviousCategoryHash = m_CategoryHashInt;
            m_LabelHashInt = ConvertFloatToInt(m_labelHash);
            m_PreviouslabelHash = m_LabelHashInt;
            ResolveSpriteToSpriteRenderer();
        }

        SpriteRenderer spriteRenderer
        {
            get { return GetComponent<SpriteRenderer>(); }
        }

        /// <summary>
        /// Set the Category and label to use
        /// </summary>
        /// <param name="category">Category to use</param>
        /// <param name="label">Label to use</param>
        public void SetCategoryAndLabel(string category, string label)
        {
            categoryHashInt = SpriteLibraryAsset.GetStringHash(category);
            m_PreviousCategoryHash = categoryHashInt;
            labelHashInt = SpriteLibraryAsset.GetStringHash(label);
            m_PreviouslabelHash = categoryHashInt;
            ResolveSpriteToSpriteRenderer();
        }

        /// <summary>
        /// Get the Category set for the SpriteResolver
        /// </summary>
        /// <returns>The Category's name</returns>
        public string GetCategory()
        {
            var returnString = "";
            var sl = spriteLibrary;
            if (sl)
                returnString = sl.GetCategoryNameFromHash(categoryHashInt);

            return returnString;
        }

        /// <summary>
        /// Get the Label set for the SpriteResolver
        /// </summary>
        /// <returns>The Label's name</returns>
        public string GetLabel()
        {
            var returnString = "";
            var sl = spriteLibrary;
            if (sl)
                returnString = sl.GetLabelNameFromHash(categoryHashInt, labelHashInt);

            return returnString;
        }

        /// <summary>
        /// Property to get the SpriteLibrary the SpriteResolver is resolving from
        /// </summary>
        public SpriteLibrary spriteLibrary => gameObject.GetComponentInParent<SpriteLibrary>(true);

        void LateUpdate()
        {
            m_CategoryHashInt = ConvertFloatToInt(m_CategoryHash);
            m_LabelHashInt = ConvertFloatToInt(m_labelHash);
            if (m_LabelHashInt != m_PreviouslabelHash || m_CategoryHashInt != m_PreviousCategoryHash)
            {
                m_PreviousCategoryHash = m_CategoryHashInt;
                m_PreviouslabelHash = m_LabelHashInt;
                ResolveSpriteToSpriteRenderer();
            }
        }

        internal Sprite GetSprite(out bool validEntry)
        {
            var lib = spriteLibrary;
            if (lib != null)
            {
                return lib.GetSprite(m_CategoryHashInt, m_LabelHashInt, out validEntry);
            }
            validEntry = false;
            return null;
        }

        /// <summary>
        /// Set the Sprite in SpriteResolver to the SpriteRenderer component that is in the same GameObject.
        /// </summary>
        public void ResolveSpriteToSpriteRenderer()
        {
            m_PreviousCategoryHash = m_CategoryHashInt;
            m_PreviouslabelHash = m_LabelHashInt;
            bool validEntry;
            var sprite = GetSprite(out validEntry);
            var sr = spriteRenderer;
            if (sr != null && (sprite != null || validEntry))
                sr.sprite = sprite;
        }

        void OnTransformParentChanged()
        {
            ResolveSpriteToSpriteRenderer();
#if UNITY_EDITOR
            spriteLibChanged = true;
#endif
        }

        int categoryHashInt
        {
            get { return m_CategoryHashInt; }
            set
            {
                m_CategoryHashInt = value;
                m_CategoryHash = ConvertIntToFloat(m_CategoryHashInt);
            }
        }

        int labelHashInt
        {
            get { return m_LabelHashInt; }
            set
            {
                m_LabelHashInt = value;
                m_labelHash = ConvertIntToFloat(m_LabelHashInt);
            }
        }

        internal unsafe static int ConvertFloatToInt(float f)
        {
            float* fp = &f;
            int* i = (int*)fp;
            return *i;
        }

        internal unsafe static float ConvertIntToFloat(int f)
        {
            int* fp = &f;
            float* i = (float*)fp;
            return *i;
        }

#if UNITY_EDITOR
        internal bool spriteLibChanged
        {
            get {return m_SpriteLibChanged;}
            set { m_SpriteLibChanged = value; }
        }
#endif
    }
}
