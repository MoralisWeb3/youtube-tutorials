using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;

namespace UnityEditor.U2D
{
    static internal class MenuItems
    {
        const int k_SpriteAssetMenuPriority = 1;
        const int k_SpriteAtlasAssetMenuPriority = k_SpriteAssetMenuPriority + 11;

        const int k_SpriteGameObjectMenuPriority = 1;
        const int k_PhysicsGameObjectMenuPriority = 2;
        const int k_SpriteMaskGameObjectMenuPriority = 6;

        [MenuItem("Assets/Create/2D/Sprites/Square", priority = k_SpriteAssetMenuPriority)]
        static void AssetsCreateSpritesSquare(MenuCommand menuCommand)
        {
            ItemCreationUtility.CreateAssetObjectFromTemplate<Texture2D>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/Square.png");
        }

        [MenuItem("Assets/Create/2D/Sprites/Circle", priority = k_SpriteAssetMenuPriority)]
        static void AssetsCreateSpritesCircle(MenuCommand menuCommand)
        {
            ItemCreationUtility.CreateAssetObjectFromTemplate<Texture2D>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/Circle.png");
        }

        [MenuItem("Assets/Create/2D/Sprites/Capsule", priority = k_SpriteAssetMenuPriority)]
        static void AssetsCreateSpritesCapsule(MenuCommand menuCommand)
        {
            ItemCreationUtility.CreateAssetObjectFromTemplate<Texture2D>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/Capsule.png");
        }

        [MenuItem("Assets/Create/2D/Sprites/Isometric Diamond", priority = k_SpriteAssetMenuPriority)]
        static void AssetsCreateSpritesIsometricDiamond(MenuCommand menuCommand)
        {
            ItemCreationUtility.CreateAssetObjectFromTemplate<Texture2D>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/IsometricDiamond.png");
        }

        [MenuItem("Assets/Create/2D/Sprites/Hexagon Flat-Top", priority = k_SpriteAssetMenuPriority)]
        static void AssetsCreateSpritesHexagonFlatTop(MenuCommand menuCommand)
        {
            ItemCreationUtility.CreateAssetObjectFromTemplate<Texture2D>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/HexagonFlat-Top.png");
        }

        [MenuItem("Assets/Create/2D/Sprites/Hexagon Pointed-Top", priority = k_SpriteAssetMenuPriority)]
        static void AssetsCreateSpritesHexagonPointedTop(MenuCommand menuCommand)
        {
            ItemCreationUtility.CreateAssetObjectFromTemplate<Texture2D>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/HexagonPointed-Top.png");
        }

        [MenuItem("Assets/Create/2D/Sprites/9-Sliced", priority = k_SpriteAssetMenuPriority)]
        static void AssetsCreateSprites9Sliced(MenuCommand menuCommand)
        {
            ItemCreationUtility.CreateAssetObjectFromTemplate<Texture2D>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/9-Sliced.png");
        }

        internal class DoCreateSpriteAtlas : ProjectWindowCallback.EndNameEditAction
        {
            public int sides;
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var spriteAtlasAsset = new SpriteAtlasAsset();

                UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { spriteAtlasAsset }, pathName, true);
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }

        static private void CreateSpriteAtlas()
        {
            var icon = EditorGUIUtility.IconContent<SpriteAtlasAsset>().image as Texture2D;
            DoCreateSpriteAtlas action = ScriptableObject.CreateInstance<DoCreateSpriteAtlas>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, "New Sprite Atlas.spriteatlasv2", icon, null);
        }

        [MenuItem("Assets/Create/2D/Sprite Atlas", priority = k_SpriteAtlasAssetMenuPriority)]
        static void AssetsCreateSpriteAtlas(MenuCommand menuCommand)
        {
            if (EditorSettings.spritePackerMode == SpritePackerMode.SpriteAtlasV2)
                CreateSpriteAtlas();
            else
                ItemCreationUtility.CreateAssetObject<SpriteAtlas>("New Sprite Atlas.spriteatlas");
        }

        static GameObject CreateSpriteRendererGameObject(string name,  string spritePath, MenuCommand menuCommand)
        {
            var go = ItemCreationUtility.CreateGameObject(name, menuCommand, new[] {typeof(SpriteRenderer)});
            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            return go;
        }

        [MenuItem("GameObject/2D Object/Sprites/Square", priority = k_SpriteGameObjectMenuPriority)]
        static void GameObjectCreateSpritesSquare(MenuCommand menuCommand)
        {
            CreateSpriteRendererGameObject("Square", "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/Square.png", menuCommand);
        }

        [MenuItem("GameObject/2D Object/Sprites/Circle", priority = k_SpriteGameObjectMenuPriority)]
        static void GameObjectCreateSpritesCircle(MenuCommand menuCommand)
        {
            CreateSpriteRendererGameObject("Circle", "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/Circle.png", menuCommand);
        }

        [MenuItem("GameObject/2D Object/Sprites/Capsule", priority = k_SpriteGameObjectMenuPriority)]
        static void GameObjectCreateSpritesCapsule(MenuCommand menuCommand)
        {
            CreateSpriteRendererGameObject("Capsule", "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/Capsule.png", menuCommand);
        }

        [MenuItem("GameObject/2D Object/Sprites/Isometric Diamond", priority = k_SpriteGameObjectMenuPriority)]
        static void GameObjectCreateSpritesIsometricDiamond(MenuCommand menuCommand)
        {
            CreateSpriteRendererGameObject("Isometric Diamond", "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/IsometricDiamond.png", menuCommand);
        }

        [MenuItem("GameObject/2D Object/Sprites/Hexagon Flat-Top", priority = k_SpriteGameObjectMenuPriority)]
        static void GameObjectCreateSpritesHexagonFlatTop(MenuCommand menuCommand)
        {
            CreateSpriteRendererGameObject("Hexagon Flat-Top", "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/HexagonFlat-Top.png", menuCommand);
        }

        [MenuItem("GameObject/2D Object/Sprites/Hexagon Pointed-Top", priority = k_SpriteGameObjectMenuPriority)]
        static void GameObjectCreateSpritesHexagonPointedTop(MenuCommand menuCommand)
        {
            CreateSpriteRendererGameObject("Hexagon Pointed-Top", "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/HexagonPointed-Top.png", menuCommand);
        }

        [MenuItem("GameObject/2D Object/Sprites/9-Sliced", priority = k_SpriteGameObjectMenuPriority)]
        static void GameObjectCreateSprites9Sliced(MenuCommand menuCommand)
        {
            var go = CreateSpriteRendererGameObject("9-Sliced", "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/9-Sliced.png", menuCommand);
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr.drawMode == SpriteDrawMode.Simple)
            {
                sr.drawMode = SpriteDrawMode.Tiled;
                sr.tileMode = SpriteTileMode.Continuous;
            }
        }

        [MenuItem("GameObject/2D Object/Physics/Static Sprite", priority = k_PhysicsGameObjectMenuPriority)]
        static void GameObjectCreatePhysicsStaticSprite(MenuCommand menuCommand)
        {
            var go = ItemCreationUtility.CreateGameObject("Static Sprite", menuCommand, new[] {typeof(SpriteRenderer), typeof(BoxCollider2D), typeof(Rigidbody2D)});
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr.sprite == null)
                sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                    "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/Square.png");
            var rigidBody = go.GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Static;
            var boxCollider2D = go.GetComponent<BoxCollider2D>();
            boxCollider2D.size = sr.sprite.rect.size / sr.sprite.pixelsPerUnit;
        }

        [MenuItem("GameObject/2D Object/Physics/Dynamic Sprite", priority = k_PhysicsGameObjectMenuPriority)]
        static void GameObjectCreatePhysicsDynamicSprite(MenuCommand menuCommand)
        {
            var go = ItemCreationUtility.CreateGameObject("Dynamic Sprite", menuCommand, new[] {typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(Rigidbody2D)});
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr.sprite == null)
                sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                    "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/Circle.png");
            var rigidBody = go.GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
        }

        [MenuItem("GameObject/2D Object/Sprite Mask", priority = k_SpriteMaskGameObjectMenuPriority)]
        static void GameObjectCreateSpriteMask(MenuCommand menuCommand)
        {
            var go = ItemCreationUtility.CreateGameObject("Sprite Mask", menuCommand, new[] {typeof(SpriteMask)});
            go.GetComponent<SpriteMask>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/CircleMask.png");
        }
    }
}
