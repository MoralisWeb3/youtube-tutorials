using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;
using UnityEngine.Serialization;

namespace UnityEditor.U2D.Animation
{
    /// <summary>
    /// Structure that defines a Sprite Library Category Label
    /// </summary>
    [Serializable]
    public struct SpriteCategoryLabel
    {
        [SerializeField]
        string m_Name;
        [SerializeField]
        string m_SpriteId;

        /// <summary>
        /// Get and set the name for the Sprite label
        /// </summary>
        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// Get and set the Sprite Id.
        /// </summary>
        public string spriteId
        {
            get { return m_SpriteId; }
            set { m_SpriteId = value; }
        }
    }

    /// <summary>
    /// Structure that defines a Sprite Library Category.
    /// </summary>
    [Serializable]
    public struct SpriteCategory
    {
        [SerializeField]
        [FormerlySerializedAs("name")]
        string m_Name;
        [SerializeField]
        List<SpriteCategoryLabel> m_Labels;

        /// <summary>
        /// Get and set the name for the Sprite Category
        /// </summary>
        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// Get and set the Sprites registered to this category.
        /// </summary>
        public List<SpriteCategoryLabel> labels
        {
            get { return m_Labels; }
            set { m_Labels = value; }
        }
    }

    /// <summary>
    /// A structure to hold a collection of SpriteCategory
    /// </summary>
    [Serializable]
    public struct SpriteCategoryList
    {
        [SerializeField]
        [FormerlySerializedAs("categories")]
        List<SpriteCategory> m_Categories;

        /// <summary>
        /// Get or set the a list of SpriteCategory
        /// </summary>
        public List<SpriteCategory> categories
        {
            get { return m_Categories; }
            set { m_Categories = value; }
        }
    }

    internal class SpriteCategoryListCacheObject : SkinningObject
    {
        [SerializeField]
        public List<SpriteCategory> categories = new List<SpriteCategory>();

        public void CopyFrom(SpriteCategoryList categoryList)
        {
            categories.Clear();
            foreach (var cat in categoryList.categories)
            {
                var spriteLibCategory = new SpriteCategory()
                {
                    name = cat.name,
                    labels = new List<SpriteCategoryLabel>(cat.labels)
                };
                categories.Add(spriteLibCategory);
            }
        }

        public SpriteCategoryList ToSpriteLibrary()
        {
            var spriteLibrary = new SpriteCategoryList();
            spriteLibrary.categories = new List<SpriteCategory>();
            foreach (var cat in categories)
            {
                var spriteLibCategory = new SpriteCategory()
                {
                    name = cat.name,
                    labels = new List<SpriteCategoryLabel>(cat.labels)
                };
                spriteLibrary.categories.Add(spriteLibCategory);
            }
            return spriteLibrary;
        }

        public void RemoveSpriteFromCategory(string sprite)
        {
            for (int i = 0; i < categories.Count; ++i)
            {
                var index = categories[i].labels.FindIndex(x => x.spriteId == sprite);
                if (index != -1)
                    categories[i].labels.RemoveAt(index);
            }
        }

        public void AddSpriteToCategory(string category, SpriteCategoryLabel label)
        {
            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(label.name))
            {
                // Remove sprite from name
                RemoveSpriteFromCategory(label.spriteId);
            }
            else
            {
                //find cateogry
                var categoryIndex = categories.FindIndex(x => x.name == category);
                if (categoryIndex == -1)
                {
                    // check if the hash might clash
                    var hash = SpriteLibraryAsset.GetStringHash(category);
                    if (categories.FindIndex(x => x.name != category && SpriteLibraryAsset.GetStringHash(x.name) == hash) != -1)
                    {
                        Debug.LogError("Unable to add Sprite to new Category due to name hash clash");
                        return;
                    }
                }
                var insertCategory = categoryIndex != -1 ? categories[categoryIndex] : new SpriteCategory() { name = category, labels = new List<SpriteCategoryLabel>() };
                if (insertCategory.labels.FindIndex(x => x.spriteId == label.spriteId) == -1)
                    insertCategory.labels.Add(label);

                // now remove everything that has this sprite
                foreach (var cat in categories)
                {
                    if (cat.name != category)
                        cat.labels.RemoveAll(x => x.spriteId == label.spriteId);
                }
                if (categoryIndex == -1)
                    categories.Add(insertCategory);
                else
                    categories[categoryIndex] = insertCategory;
            }
        }

        public void ChangeSpriteLabelName(string labelname, string sprite)
        {
            // find name which contain sprite
            var categoryIndex = -1;
            var spriteIndex = -1;
            for (int i = 0; i < categories.Count; ++i)
            {
                spriteIndex = categories[i].labels.FindIndex(x => x.spriteId == sprite);
                if (spriteIndex != -1)
                {
                    categoryIndex = i;
                    break;
                }
            }

            if (categoryIndex != -1 && spriteIndex != -1)
            {
                var cat = categories[categoryIndex];
                if (string.IsNullOrEmpty(labelname))
                {
                    cat.labels.RemoveAt(spriteIndex);
                }
                else
                {
                    var label = cat.labels[spriteIndex];
                    label.name = labelname;
                    cat.labels[spriteIndex] = label;
                }
            }
        }
    }

    /// <summary>An interface that allows Sprite Editor Modules to edit Sprite Library data for user custom importer.</summary>
    /// <remarks>Implement this interface for [[ScriptedImporter]] to leverage on Sprite Editor Modules to edit Sprite Library data.</remarks>
    public interface ISpriteLibDataProvider
    {
        /// <summary>
        /// Returns the SpriteCategoryList structure that represents the Sprite Library data.
        /// </summary>
        /// <returns>SpriteCategoryList data</returns>
        SpriteCategoryList GetSpriteCategoryList();


        /// <summary>
        /// Sets the SpriteCategoryList structure that represents the Sprite Library data to the data provider
        /// </summary>
        /// <param name="spriteCategoryList">Data to set</param>
        void SetSpriteCategoryList(SpriteCategoryList spriteCategoryList);
    }
}
