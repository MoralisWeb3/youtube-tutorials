using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Assertions;

namespace UnityEngine.Experimental.U2D.Animation
{
    internal interface INameHash
    {
        string name { get; set; }
        int hash { get; }
    }

    [Serializable]
    internal class Categorylabel : INameHash
    {
        [SerializeField]
        string m_Name;
        [SerializeField]
        [HideInInspector]
        int m_Hash;
        [SerializeField]
        Sprite m_Sprite;

        public string name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                m_Hash = SpriteLibraryAsset.GetStringHash(m_Name);
            }
        }
        public int hash { get { return m_Hash; } }
        public Sprite sprite {get { return m_Sprite; } set { m_Sprite = value; }}
        public void UpdateHash()
        {
            m_Hash = SpriteLibraryAsset.GetStringHash(m_Name);
        }
    }

    [Serializable]
    internal class SpriteLibCategory : INameHash
    {
        [SerializeField]
        string m_Name;
        [SerializeField]
        int m_Hash;
        [SerializeField]
        List<Categorylabel> m_CategoryList;

        public string name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                m_Hash = SpriteLibraryAsset.GetStringHash(m_Name);
            }
        }

        public int hash { get { return m_Hash; } }

        public List<Categorylabel> categoryList
        {
            get { return m_CategoryList; }
            set { m_CategoryList = value; }
        }

        public void UpdateHash()
        {
            m_Hash = SpriteLibraryAsset.GetStringHash(m_Name);
            foreach (var s in m_CategoryList)
                s.UpdateHash();
        }

        internal void ValidateLabels()
        {
            SpriteLibraryAsset.RenameDuplicate(m_CategoryList,
                (originalName, newName)
                =>
                {
                    Debug.LogWarning(string.Format("Label {0} renamed to {1} due to hash clash", originalName, newName));
                });
        }
    }

    /// <summary>
    /// A custom Asset that stores Sprites grouping
    /// </summary>
    /// <Description>
    /// Sprites are grouped under a given category as categories. Each category and label needs to have
    /// a name specified so that it can be queried.
    /// </Description>
    [CreateAssetMenu(order = 9, menuName = "2D/Sprite Library Asset")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.2d.animation@latest/index.html?subfolder=/manual/SLAsset.html")]
    public class SpriteLibraryAsset : ScriptableObject
    {
        [SerializeField]
        private List<SpriteLibCategory> m_Labels = new List<SpriteLibCategory>();

        internal List<SpriteLibCategory> categories
        {
            get
            {
                return m_Labels;
            }
            set
            {
                m_Labels = value;
                ValidateCategories();
            }
        }

        internal Sprite GetSprite(int categoryHash, int labelHash)
        {
            var category = m_Labels.FirstOrDefault(x => x.hash == categoryHash);
            if (category != null)
            {
                var spritelabel = category.categoryList.FirstOrDefault(x => x.hash == labelHash);
                if (spritelabel != null)
                {
                    return spritelabel.sprite;
                }
            }

            return null;
        }

        internal Sprite GetSprite(int categoryHash, int labelHash, out bool validEntry)
        {
            SpriteLibCategory category = null;
            for (int i = 0; i < m_Labels.Count; ++i)
            {
                if (m_Labels[i].hash == categoryHash)
                {
                    category = m_Labels[i];
                    break;
                }
            }
            
            if (category != null)
            {
                Categorylabel spritelabel = null;
                for (int i = 0; i < category.categoryList.Count; ++i)
                {
                    if (category.categoryList[i].hash == labelHash)
                    {
                        spritelabel = category.categoryList[i];
                        break;
                    }
                }
                if (spritelabel != null)
                {
                    validEntry = true;
                    return spritelabel.sprite;
                }
            }
            validEntry = false;
            return null;
        }

        /// <summary>
        /// Returns the Sprite registered in the Asset given the Category and Label value
        /// </summary>
        /// <param name="category">Category string value</param>
        /// <param name="label">Label string value</param>
        /// <returns></returns>
        public Sprite GetSprite(string category, string label)
        {
            var categoryHash = SpriteLibraryAsset.GetStringHash(category);
            var labelHash = SpriteLibraryAsset.GetStringHash(label);
            return GetSprite(categoryHash, labelHash);
        }

        /// <summary>
        /// Return all the Category names of the Sprite Library Asset that is associated.
        /// </summary>
        /// <returns>A Enumerable string value representing the name</returns>
        public IEnumerable<string> GetCategoryNames()
        {
            return m_Labels.Select(x => x.name);
        }

        /// <summary>
        /// (Obsolete) Returns the labels' name for the given name
        /// </summary>
        /// <param name="category">Category name</param>
        /// <returns>A Enumerable string representing labels' name</returns>
        [Obsolete("GetCategorylabelNames has been deprecated. Please use GetCategoryLabelNames (UnityUpgradable) -> GetCategoryLabelNames(*)")]
        public IEnumerable<string> GetCategorylabelNames(string category)
        {
            return GetCategoryLabelNames(category);
        }

        /// <summary>
        /// Returns the labels' name for the given name
        /// </summary>
        /// <param name="category">Category name</param>
        /// <returns>A Enumerable string representing labels' name</returns>
        public IEnumerable<string> GetCategoryLabelNames(string category)
        {
            var label = m_Labels.FirstOrDefault(x => x.name == category);
            return label == null ? new string[0] : label.categoryList.Select(x => x.name);
        }

        internal string GetCategoryNameFromHash(int hash)
        {
            var label = m_Labels.FirstOrDefault(x => x.hash == hash);
            return label == null ? "" : label.name;
        }

        /// <summary>
        /// Add or replace and existing Sprite into the given Category and Label
        /// </summary>
        /// <param name="sprite">Sprite to add</param>
        /// <param name="category">Category to add the Sprite to</param>
        /// <param name="label">Label of the Category to add the Sprite to</param>
        public void AddCategoryLabel(Sprite sprite, string category, string label)
        {
            category = category.Trim();
            label = label.Trim();
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(label))
            {
                Debug.LogError("Cannot add label with empty or null Category or label string");
            }
            var catHash = SpriteLibraryAsset.GetStringHash(category);
            Categorylabel categorylabel = null;
            SpriteLibCategory libCategory = null;
            libCategory = m_Labels.FirstOrDefault(x => x.hash == catHash);

            if (libCategory != null)
            {
                Assert.AreEqual(libCategory.name, category, "Category string  hash clashes with another existing Category. Please use another string");

                var labelHash = SpriteLibraryAsset.GetStringHash(label);
                categorylabel = libCategory.categoryList.FirstOrDefault(y => y.hash == labelHash);
                if (categorylabel != null)
                {
                    Assert.AreEqual(categorylabel.name, label, "Label string hash clashes with another existing label. Please use another string");
                    categorylabel.sprite = sprite;
                }
                else
                {
                    categorylabel = new Categorylabel()
                    {
                        name = label,
                        sprite = sprite
                    };
                    libCategory.categoryList.Add(categorylabel);
                }
            }
            else
            {
                var slc = new SpriteLibCategory()
                {
                    categoryList = new List<Categorylabel>()
                    {
                        new Categorylabel()
                        {
                            name = label,
                            sprite = sprite
                        }
                    },
                    name = category
                };
                m_Labels.Add(slc);
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// Remove a Label from a given Category
        /// </summary>
        /// <param name="category">Category to remove from</param>
        /// <param name="label">Label to remove</param>
        /// <param name="deleteCategory">Indicate to remove the Category if it is empty</param>
        public void RemoveCategoryLabel(string category, string label, bool deleteCategory)
        {
            var catHash = SpriteLibraryAsset.GetStringHash(category);
            SpriteLibCategory libCategory = null;
            libCategory = m_Labels.FirstOrDefault(x => x.hash == catHash);

            if (libCategory != null)
            {
                var labelHash = SpriteLibraryAsset.GetStringHash(label);
                libCategory.categoryList.RemoveAll(x => x.hash == labelHash);
                if (deleteCategory && libCategory.categoryList.Count == 0)
                    m_Labels.RemoveAll(x => x.hash == libCategory.hash);
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        internal string GetLabelNameFromHash(int categoryHas, int labelHash)
        {
            var labels = m_Labels.FirstOrDefault(x => x.hash == categoryHas);
            if (labels != null)
            {
                var label = labels.categoryList.FirstOrDefault(x => x.hash == labelHash);
                return label == null ? "" : label.name;
            }
            return "";
        }

        internal void UpdateHashes()
        {
            foreach (var e in m_Labels)
                e.UpdateHash();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        internal void ValidateCategories()
        {
            RenameDuplicate(m_Labels, (originalName, newName)
                =>
                {
                    Debug.LogWarning(string.Format("Category {0} renamed to {1} due to hash clash", originalName, newName));
                });
            for (int i = 0; i < m_Labels.Count; ++i)
            {
                // Verify categories have no hash clash
                var category = m_Labels[i];

                // Verify labels have no clash
                category.ValidateLabels();
            }
        }

        internal static void RenameDuplicate(IEnumerable<INameHash> nameHashList, Action<string, string> onRename)
        {
            const int k_IncrementMax = 1000;
            for (int i = 0; i < nameHashList.Count(); ++i)
            {
                // Verify categories have no hash clash
                var category = nameHashList.ElementAt(i);
                var categoriesClash = nameHashList.Where(x => (x.hash == category.hash || x.name == category.name) && x != category);
                int increment = 0;
                for (int j = 0; j < categoriesClash.Count(); ++j)
                {
                    var categoryClash = categoriesClash.ElementAt(j);

                    while (increment < k_IncrementMax)
                    {
                        var name = categoryClash.name;
                        name = string.Format("{0}_{1}", name, increment);
                        var nameHash = SpriteLibraryAsset.GetStringHash(name);
                        var exist = nameHashList.FirstOrDefault(x => (x.hash == nameHash || x.name == name) && x != categoryClash);
                        if (exist == null)
                        {
                            onRename(categoryClash.name, name);
                            categoryClash.name = name;
                            break;
                        }
                        ++increment;
                    }
                }
            }
        }

        // Allow delegate override for test
        internal static Func<string, int> GetStringHash = Default_GetStringHash;
        internal static int Default_GetStringHash(string value)
        {
#if DEBUG_GETSTRINGHASH_CLASH
            if (value == "abc" || value == "123")
                value = "abc";
#endif
            var hash = Animator.StringToHash(value);
            var bytes = BitConverter.GetBytes(hash);
            var exponentialBit = BitConverter.IsLittleEndian ? 3 : 1;
            if (bytes[exponentialBit] == 0xFF)
                bytes[exponentialBit] -= 1;
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
