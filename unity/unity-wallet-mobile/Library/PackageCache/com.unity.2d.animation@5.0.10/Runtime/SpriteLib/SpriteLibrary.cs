using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEngine.Experimental.U2D.Animation
{
    /// <summary>
    /// Component that holds a Sprite Library Asset. The component is used by SpriteResolver Component to query for Sprite based on Category and Index
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("2D Animation/Sprite Library (Experimental)")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.animation@latest/index.html?subfolder=/manual/SLAsset.html%23sprite-library-component")]
    public class SpriteLibrary : MonoBehaviour
    {
        internal struct StringAndHash
        {
            public string name;
            public readonly int hash;

            public StringAndHash(string name)
            {
                this.name = name;
                hash = SpriteLibraryAsset.GetStringHash(name);
            }

            public StringAndHash(int hash)
            {
                name = "";
                this.hash = hash;
            }

            public static bool operator==(StringAndHash l, StringAndHash r)
            {
                return l.Equals(r);
            }

            public static bool operator!=(StringAndHash l, StringAndHash r)
            {
                return !l.Equals(r);
            }

            public override bool Equals(object obj)
            {
                if (obj == null || this.GetType() != obj.GetType())
                    return false;

                return this.Equals((StringAndHash)obj);
            }

            private bool Equals(StringAndHash p)
            {
                // If run-time types are not exactly the same, return false.
                if (this.GetType() != p.GetType())
                    return false;

                return (hash == p.hash) || (name == p.name);
            }

            public override int GetHashCode()
            {
                return hash;
            }
        }

        [SerializeField]
        private SpriteLibraryAsset m_SpriteLibraryAsset;

        Dictionary<StringAndHash, Dictionary<StringAndHash, Sprite>> m_Overrides = new Dictionary<StringAndHash, Dictionary<StringAndHash, Sprite>>();

        /// <summary>Get or Set the current SpriteLibraryAsset to use </summary>
        public SpriteLibraryAsset spriteLibraryAsset
        {
            set
            {
                if (m_SpriteLibraryAsset != value)
                {
                    m_SpriteLibraryAsset = value;
                    RefreshSpriteResolvers();
                }
            }
            get { return m_SpriteLibraryAsset; }
        }

        /// <summary>
        /// Return the Sprite that is registered for the given Category and Label for the SpriteLibrary
        /// </summary>
        /// <param name="category">Category name</param>
        /// <param name="label">Label name</param>
        /// <returns>Sprite associated to the name and index</returns>
        public Sprite GetSprite(string category, string label)
        {
            var categoryHash = SpriteLibraryAsset.GetStringHash(category);
            var labelHash = SpriteLibraryAsset.GetStringHash(label);
            return GetSprite(categoryHash, labelHash);
        }

        internal Sprite GetSprite(int categoryHash, int labelHash)
        {
            return GetSprite(categoryHash, labelHash, out _);
        }

        internal Sprite GetSprite(int categoryHash, int labelHash, out bool validEntry)
        {
            validEntry = false;
            var cat = new StringAndHash(categoryHash);
            var label = new StringAndHash(labelHash);
            if (m_Overrides.ContainsKey(cat) && m_Overrides[cat].ContainsKey(label))
            {
                validEntry = true;
                return m_Overrides[cat][label];
            }
            return m_SpriteLibraryAsset == null ? null : m_SpriteLibraryAsset.GetSprite(categoryHash, labelHash, out validEntry);
        }

        internal string GetCategoryNameFromHash(int categoryHash)
        {
            var key = m_Overrides.Keys.FirstOrDefault(x => x.hash == categoryHash);
            if (key != default)
                return key.name;
            return m_SpriteLibraryAsset == null ? "" : m_SpriteLibraryAsset.GetCategoryNameFromHash(categoryHash);
        }

        internal string GetLabelNameFromHash(int categoryHash, int labelHash)
        {
            var overrides = GetCategoryOverride(new StringAndHash(categoryHash), false);
            var label = overrides.Keys.FirstOrDefault(x => x.hash == labelHash);
            if (label != default)
                return label.name;
            return m_SpriteLibraryAsset == null ? "" : m_SpriteLibraryAsset.GetLabelNameFromHash(categoryHash, labelHash);
        }

        private Dictionary<StringAndHash, Sprite> GetCategoryOverride(string category, bool addToList)
        {
            return GetCategoryOverride(new StringAndHash(category), addToList);
        }

        private Dictionary<StringAndHash, Sprite> GetCategoryOverride(StringAndHash category, bool addToList)
        {
            Dictionary<StringAndHash, Sprite> label;
            if (m_Overrides.ContainsKey(category))
                label = m_Overrides[category];
            else
                label = new Dictionary<StringAndHash, Sprite>();


            if (addToList && !m_Overrides.ContainsKey(category))
            {
                if (string.IsNullOrEmpty(category.name))
                    Debug.LogWarning("Adding override category with no name");
                m_Overrides.Add(category, label);
            }

            return label;
        }

        private void AddSpriteToOverride(Dictionary<StringAndHash, Sprite> overrides, StringAndHash label, Sprite sprite)
        {
            if (overrides.ContainsKey(label))
                overrides[label] = sprite;
            else
                overrides.Add(label, sprite);
            RefreshSpriteResolvers();
        }

        /// <summary>
        /// Add or replace an override when querying for the given Category and Label from a SpriteLibraryAsset
        /// </summary>
        /// <param name="spriteLib">Sprite Library Asset to query</param>
        /// <param name="category">Category name from the Sprite Library Asset to add override</param>
        /// <param name="label">Label name to add override</param>
        public void AddOverride(SpriteLibraryAsset spriteLib, string category, string label)
        {
            var sprite = spriteLib.GetSprite(category, label);
            var overridelabel = GetCategoryOverride(category, true);
            AddSpriteToOverride(overridelabel, new StringAndHash(label), sprite);
        }

        /// <summary>
        /// Add or replace an override when querying for the given Category. All the categories in the Category will be added.
        /// </summary>
        /// <param name="spriteLib">Sprite Library Asset to query</param>
        /// <param name="category">Category name from the Sprite Library Asset to add override</param>
        public void AddOverride(SpriteLibraryAsset spriteLib, string category)
        {
            var categoryHash = SpriteLibraryAsset.GetStringHash(category);
            var cat = spriteLib.categories.FirstOrDefault(x => x.hash == categoryHash);
            if (cat != null)
            {
                var label = GetCategoryOverride(category, true);
                for (int i = 0; i < cat.categoryList.Count; ++i)
                {
                    AddSpriteToOverride(label, new StringAndHash(cat.categoryList[i].name), cat.categoryList[i].sprite);
                }
            }
        }

        /// <summary>
        /// Add or replace an override when querying for the given Category and Label.
        /// </summary>
        /// <param name="sprite">Sprite to override to</param>
        /// <param name="category">Category name to override</param>
        /// <param name="label">Label name to override</param>
        public void AddOverride(Sprite sprite, string category, string label)
        {
            var overridelabel = GetCategoryOverride(category, true);
            AddSpriteToOverride(overridelabel, new StringAndHash(label), sprite);
        }

        /// <summary>
        /// Remove all Sprite Library override for a given category
        /// </summary>
        /// <param name="category">Category overrides to remove</param>
        public void RemoveOverride(string category)
        {
            var hash = new StringAndHash(SpriteLibraryAsset.GetStringHash(category));
            m_Overrides.Remove(hash);
            RefreshSpriteResolvers();
        }

        /// <summary>
        /// Remove Sprite Library override for a given category and label
        /// </summary>
        /// <param name="category">Category to remove</param>
        /// <param name="label">Label to remove</param>
        public void RemoveOverride(string category, string label)
        {
            var catlabel = GetCategoryOverride(category, false);
            if (catlabel != null)
            {
                catlabel.Remove(new StringAndHash(SpriteLibraryAsset.GetStringHash(label)));
                RefreshSpriteResolvers();
            }
        }

        /// <summary>
        /// Method to check if a Category and Label pair has an override
        /// </summary>
        /// <param name="category">Category name</param>
        /// <param name="label">Label name</param>
        /// <returns>True if override exist, false otherwise</returns>
        public bool HasOverride(string category, string label)
        {
            var catOverride = GetCategoryOverride(category, false);
            if (catOverride != null)
                return catOverride.ContainsKey(new StringAndHash(label));
            return false;
        }

        internal List<SpriteLibCategory> labels
        {
            get { return m_SpriteLibraryAsset != null ? m_SpriteLibraryAsset.categories : new List<SpriteLibCategory>(); }
        }

        /// <summary>
        /// Request SpriteResolver components that are in the same hierarchy to refresh
        /// </summary>
        public void RefreshSpriteResolvers()
        {
            var spriteResolvers = GetComponentsInChildren<SpriteResolver>();
            foreach (var sr in spriteResolvers)
            {
                sr.ResolveSpriteToSpriteRenderer();
#if UNITY_EDITOR
                sr.spriteLibChanged = true;
#endif
            }
        }
    }
}
