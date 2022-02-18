using System;
using UnityEngine;

using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.U2D.Animation
{
    /// <summary>An interface that allows Sprite Editor Modules to edit Character data for user custom importer.</summary>
    /// <remarks>Implement this interface for [[ScriptedImporter]] to leverage on Sprite Editor Modules to edit Character data.</remarks>
    [MovedFrom("UnityEditor.U2D.Experimental.Animation")]
    public interface ICharacterDataProvider
    {
        /// <summary>
        /// Returns the CharacterData structure that represents the Character composition.
        /// </summary>
        /// <returns>CharacterData data</returns>
        CharacterData GetCharacterData();
        /// <summary>
        /// Sets the CharacterData structure that represents to the data provider
        /// </summary>
        /// <param name="characterData">CharacterData to set</param>
        void SetCharacterData(CharacterData characterData);
    }

    /// <summary>
    /// Data structure that represents a character setup
    /// </summary>
    [Serializable]
    [MovedFrom("UnityEditor.U2D.Experimental.Animation")]
    public struct CharacterData
    {
        /// <summary>
        /// SpriteBones influencing the Character
        /// </summary>
        public UnityEngine.U2D.SpriteBone[] bones;
        /// <summary>
        /// Parts of the character
        /// </summary>
        public CharacterPart[] parts;
        /// <summary>
        /// The dimension of the character required
        /// </summary>
        public Vector2Int dimension;
        /// <summary>
        /// Character grouping information
        /// </summary>
        public CharacterGroup[] characterGroups;
    }

    internal interface ICharacterOrder
    {
        int order { get; set;}
    }
    
    /// <summary>
    /// Data structure representing CharacterPart grouping
    /// </summary>
    [Serializable]
    [MovedFrom("UnityEditor.U2D.Experimental.Animation")]
    public struct CharacterGroup : ICharacterOrder
    {
        /// <summary>
        /// Name of the CharacterGroup
        /// </summary>
        public string name;
        /// <summary>
        /// The parent group index it belongs to. Set to -1 if does not have a parent.
        /// </summary>
        public int parentGroup;

        [SerializeField]
        int m_Order;

        /// <summary>
        /// The order of the group in the list
        /// </summary>
        public int order
        {
            get => m_Order;
            set => m_Order = value;
        }
    }

    /// <summary>
    /// Data structure representing a character part
    /// </summary>
    [Serializable]
    [MovedFrom("UnityEditor.U2D.Experimental.Animation")]
    public struct CharacterPart : ICharacterOrder
    {
        /// <summary>
        /// Position for the Sprite in the character
        /// </summary>
        public RectInt spritePosition;
        /// <summary>
        /// Sprite ID
        /// </summary>
        public string spriteId;
        /// <summary>
        /// Bones influencing the Sprite
        /// </summary>
        public int[] bones;
        /// <summary>
        /// CharacterGroup that the part belongs to
        /// </summary>
        public int parentGroup;
        
        [SerializeField]
        int m_Order;

        /// <summary>
        /// The order of the part in the list
        /// </summary>
        public int order
        {
            get => m_Order;
            set => m_Order = value;
        }
    }
}
