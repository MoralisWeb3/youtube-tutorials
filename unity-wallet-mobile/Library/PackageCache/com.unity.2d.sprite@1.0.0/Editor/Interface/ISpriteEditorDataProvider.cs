using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityObject = UnityEngine.Object;

namespace UnityEditor.U2D.Sprites
{
    /// <summary>An interface that allows Sprite Editor Window to edit Sprite data for user custom importer.</summary>
    /// <remarks>Implement this interface for [[ScriptedImporter]] to leverage on Sprite Editor Window to edit Sprite data.</remarks>
    public interface ISpriteEditorDataProvider
    {
        /// <summary>SpriteImportMode to indicate how Sprite data will be imported.</summary>
        SpriteImportMode spriteImportMode { get; }
        /// <summary>The number of pixels in the sprite that correspond to one unit in world space.</summary>
        float pixelsPerUnit { get; }
        /// <summary>The object that this data provider is acquiring its data from.</summary>
        UnityObject targetObject { get; }
        /// <summary>Returns an array of SpriteRect representing Sprite data the provider has.</summary>
        /// <returns>Array of SpriteRect.</returns>
        SpriteRect[] GetSpriteRects();
        /// <summary>Sets the data provider's current SpriteRect.</summary>
        /// <param name="spriteRects">Updated array of SpriteRect.</param>
        void SetSpriteRects(SpriteRect[] spriteRects);
        /// <summary>Applying any changed data.</summary>
        void Apply();
        /// <summary>Allows the data provider to initialize any data if needed.</summary>
        void InitSpriteEditorDataProvider();
        /// <summary>Gets other data providers that might be supported by ISpriteEditorDataProvider.targetObject.</summary>
        /// <typeparam name="T">The data provider type to acquire.</typeparam>
        /// <returns>Data provider type.</returns>
        T GetDataProvider<T>() where T : class;
        /// <summary>Queries if ISpriteEditorDataProvider.targetObject supports the data provider type.</summary>
        /// <param name="type">Data provider type.</param>
        /// <returns>True if supports, false otherwise.</returns>
        bool HasDataProvider(Type type);
    }

    /// <summary>
    /// Data Provider interface that deals with Sprite Bone data.
    /// </summary>
    public interface ISpriteBoneDataProvider
    {
        /// <summary>
        /// Returns the list of SpriteBone for the corresponding Sprite ID.
        /// </summary>
        /// <param name="guid">Sprite ID.</param>
        /// <returns>The list of SpriteBone associated with the Sprite</returns>
        List<SpriteBone> GetBones(GUID guid);

        /// <summary>Sets a new set of SpriteBone for the corresponding Sprite ID.</summary>
        /// <param name = "guid" > Sprite ID.</param>
        /// <param name = "bones" > List of SpriteBone to associate with the Sprite.</param>
        void SetBones(GUID guid, List<SpriteBone> bones);
    }

    /// <summary>Data provider that provides the outline data for SpriteRect.</summary>
    /// <remarks>The outline data is used to tessellate a Sprite's mesh.</remarks>
    public interface ISpriteOutlineDataProvider
    {
        /// <summary>Given a GUID, returns the outline data used for tessellating the SpriteRect.</summary>
        /// <param name="guid">GUID of the SpriteRect.</param>
        /// <returns>Outline data for theSpriteRect.</returns>
        List<Vector2[]> GetOutlines(GUID guid);
        /// <summary>Given a GUID, sets the outline data used for tessellating the SpriteRect.</summary>
        /// <param name="guid">GUID of the SpriteRect.</param>
        /// <param name="data">Outline data for theSpriteRect.</param>
        void SetOutlines(GUID guid, List<Vector2[]> data);
        /// <summary>Given a GUID, returns the tessellation detail.Tessellation value should be between 0 to 1.</summary>
        /// <param name = "guid" > GUID of the SpriteRect.</param>
        /// <returns>The tessellation value.</returns>
        float GetTessellationDetail(GUID guid);
        /// <summary>Given a GUID, sets the tessellation detail.Tessellation value should be between 0 to 1.</summary>
        /// <param name = "guid" > GUID of the SpriteRect.</param>
        /// <param name="value">The tessellation value.</param>
        void SetTessellationDetail(GUID guid, float value);
    }

    /// <summary>Data provider that provides the Physics outline data for SpriteRect.</summary>
    /// <remarks>Uses the outline data to generate the Sprite's Physics shape for Polygon Collider 2D.</remarks>
    public interface ISpritePhysicsOutlineDataProvider
    {
        /// <summary>Given a GUID, returns the Physics outline data used for the SpriteRect.</summary>
        /// <param name="guid">GUID of the SpriteRect.</param>
        /// <returns>Physics outline data for the SpriteRect.</returns>
        List<Vector2[]> GetOutlines(GUID guid);
        /// <summary>Given a GUID, sets the Physics outline data used for the SpriteRect.</summary>
        /// <param name="guid">GUID of the SpriteRect.</param>
        /// <param name="data">Physics outline data for the SpriteRect.</param>
        void SetOutlines(GUID guid, List<Vector2[]> data);
        /// <summary>Given a GUID, returns the tessellation detail.Tessellation value should be between 0 to 1.</summary>
        /// <param name = "guid" > GUID of the SpriteRect.</param>
        /// <returns>The tessellation value.</returns>
        float GetTessellationDetail(GUID guid);
        /// <summary>Given a GUID, sets the tessellation detail.Tessellation value should be between 0 to 1.</summary>
        /// <param name = "guid" > GUID of the SpriteRect.</param>
        /// <param name="value">The tessellation value.</param>
        void SetTessellationDetail(GUID guid, float value);
    }

    /// <summary>Data provider that provides texture data needed for Sprite Editor Window.</summary>
    public interface ITextureDataProvider
    {
        /// <summary>Texture2D representation of the data provider.</summary>
        Texture2D texture { get; }
        /// <summary>Texture2D that represents the preview for ITextureDataProvider.texture.</summary>
        Texture2D previewTexture { get; }
        /// <summary>The actual width and height of the texture data.</summary>
        /// <param name="width">Out value for width.</param>
        /// <param name="height">Out value for height.</param>
        void GetTextureActualWidthAndHeight(out int width , out int height);
        /// <summary>Readable version of ITextureProvider.texture.</summary>
        /// <returns>Texture2D that is readable.</returns>
        Texture2D GetReadableTexture2D();
    }

    /// <summary>Data provider that provides secoondary texture data needed for Sprite Editor Window.</summary>
    public interface ISecondaryTextureDataProvider
    {
        /// <summary>
        /// Get set method for an array of SecondarySpriteTexture in the Data Provider
        /// </summary>
        SecondarySpriteTexture[] textures { get; set; }
    }

    /// <summary>A structure that contains meta data about vertices in a Sprite.</summary>
    [Serializable]
    public struct Vertex2DMetaData
    {
        /// <summary>The position of the vertex.</summary>
        public Vector2 position;
        /// <summary>The BoneWeight of the vertex.</summary>
        public BoneWeight boneWeight;
    }

    /// <summary>Data Provider interface that deals with Sprite mesh data.</summary>
    public interface ISpriteMeshDataProvider
    {
        /// <summary>Returns the list of vertex datas for the corresponding Sprite ID.</summary>
        /// <param name = "guid" > Sprite ID.</param>
        Vertex2DMetaData[] GetVertices(GUID guid);
        /// <summary>Sets a new list of vertices for the corresponding Sprite ID.</summary>
        /// <param name = "guid" > Sprite ID.</param>
        void SetVertices(GUID guid, Vertex2DMetaData[] vertices);
        /// <summary>Returns the list of mesh index for the corresponding Sprite ID.</summary>
        /// <param name = "guid" > Sprite ID.</param>
        int[] GetIndices(GUID guid);
        /// <summary>Sets a new list of indices for the corresponding Sprite ID.</summary>
        /// <param name = "guid" > Sprite ID.</param>
        void SetIndices(GUID guid, int[] indices);
        /// <summary>Returns the list of mesh edges for the corresponding Sprite ID.</summary>
        /// <param name = "guid" > Sprite ID.</param>
        Vector2Int[] GetEdges(GUID guid);
        /// <summary>Sets a new list of edges for the corresponding Sprite ID.</summary>
        /// <param name = "guid" > Sprite ID.</param>
        void SetEdges(GUID guid, Vector2Int[] edges);
    }
}
