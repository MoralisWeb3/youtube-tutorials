using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace UnityEditor.U2D.SpriteShape
{
    public class SpriteShapeEditorUtility
    {
        private static class Contents
        {
            public static readonly string createSpriteShapeString = "Create Sprite Shape";
            public static readonly string newSpriteShapeString = "SpriteShape";
        }

        public const float kMaxSideSize = 2.0f;

        public static UnityEngine.U2D.SpriteShape CreateSpriteShapeAsset()
        {
            UnityEngine.U2D.SpriteShape spriteShape = ScriptableObject.CreateInstance<UnityEngine.U2D.SpriteShape>();
            ProjectWindowUtil.CreateAsset(spriteShape, "New SpriteShapeProfile.asset");
            Selection.activeObject = spriteShape;

            SpriteShapeEditorAnalytics.instance.eventBus.spriteShapeEvent.Invoke(spriteShape);
            return spriteShape;
        }

        public static SpriteShapeController CreateSpriteShapeController(UnityEngine.U2D.SpriteShape shape)
        {
            var objName = "New SpriteShapeController";
            GameObject gameObject = new GameObject(objName, typeof(SpriteShapeController));
            SpriteShapeController spriteShapeController = gameObject.GetComponent<SpriteShapeController>();
            spriteShapeController.spline.Clear();
            if (shape != null)
                objName = shape.name;
            gameObject.name = GameObjectUtility.GetUniqueNameForSibling(gameObject.transform.parent, objName);
            SpriteShapeEditorAnalytics.instance.eventBus.spriteShapeRendererEvent.Invoke(gameObject.GetComponent<SpriteShapeRenderer>());
            return spriteShapeController;
        }

        public static SpriteShapeController CreateSpriteShapeControllerFromSelection()
        {
            var objName = "New SpriteShapeController";
            GameObject gameObject = new GameObject(objName, typeof(SpriteShapeController));
            SpriteShapeController spriteShapeController = gameObject.GetComponent<SpriteShapeController>();
            if (Selection.activeObject is UnityEngine.U2D.SpriteShape)
            {
                spriteShapeController.spriteShape = (UnityEngine.U2D.SpriteShape)Selection.activeObject;
                objName = spriteShapeController.spriteShape.name;
            }
            else if (Selection.activeObject is GameObject)
            {
                var activeGO = (GameObject)Selection.activeObject;
                var prefabType = PrefabUtility.GetPrefabAssetType(activeGO);
                if (prefabType != PrefabAssetType.Regular && prefabType != PrefabAssetType.Model)
                {
                    GameObjectUtility.SetParentAndAlign(gameObject, activeGO);
                }
            }
            gameObject.name = GameObjectUtility.GetUniqueNameForSibling(gameObject.transform.parent, objName);
            Undo.RegisterCreatedObjectUndo(gameObject, Contents.createSpriteShapeString);
            Selection.activeGameObject = gameObject;
            spriteShapeController.spline.Clear();
            SpriteShapeEditorAnalytics.instance.eventBus.spriteShapeRendererEvent.Invoke(gameObject.GetComponent<SpriteShapeRenderer>());
            return spriteShapeController;
        }

        public static void SetShapeFromAsset(SpriteShapeController spriteShapeController)
        {
            UnityEngine.U2D.SpriteShape spriteShape = spriteShapeController.spriteShape;

            if (!spriteShape)
            {
                SpriteShapeEditorUtility.SetToSquare(spriteShapeController);
                return;
            }

            if (spriteShape.angleRanges.Count == 1 && spriteShape.angleRanges[0].end - spriteShape.angleRanges[0].start == 360f)
                SpriteShapeEditorUtility.SetToLine(spriteShapeController);
            else if (spriteShape.angleRanges.Count < 8)
                SpriteShapeEditorUtility.SetToSquare(spriteShapeController);
            else
                SpriteShapeEditorUtility.SetToOctogon(spriteShapeController);
        }

        static void SetToSquare(SpriteShapeController spriteShapeController)
        {
            spriteShapeController.spline.Clear();
            spriteShapeController.spline.InsertPointAt(0, new Vector3(-kMaxSideSize, -kMaxSideSize, 0));
            spriteShapeController.spline.InsertPointAt(1, new Vector3(-kMaxSideSize, kMaxSideSize, 0));
            spriteShapeController.spline.InsertPointAt(2, new Vector3(kMaxSideSize, kMaxSideSize, 0));
            spriteShapeController.spline.InsertPointAt(3, new Vector3(kMaxSideSize, -kMaxSideSize, 0));
        }

        static void SetToLine(SpriteShapeController spriteShapeController)
        {
            spriteShapeController.spline.Clear();
            spriteShapeController.spline.InsertPointAt(0, new Vector3(-kMaxSideSize, 0.0f, 0));
            spriteShapeController.spline.InsertPointAt(1, new Vector3(kMaxSideSize, 0.0f, 0));
            spriteShapeController.spline.isOpenEnded = true;
        }

        static void SetToOctogon(SpriteShapeController spriteShapeController)
        {
            float kMaxSideSizeHalf = kMaxSideSize * 0.5f;

            spriteShapeController.spline.Clear();
            spriteShapeController.spline.InsertPointAt(0, new Vector3(-kMaxSideSizeHalf, -kMaxSideSize, 0));
            spriteShapeController.spline.InsertPointAt(1, new Vector3(-kMaxSideSize, -kMaxSideSizeHalf, 0));
            spriteShapeController.spline.InsertPointAt(2, new Vector3(-kMaxSideSize, kMaxSideSizeHalf, 0));
            spriteShapeController.spline.InsertPointAt(3, new Vector3(-kMaxSideSizeHalf, kMaxSideSize, 0));
            spriteShapeController.spline.InsertPointAt(4, new Vector3(kMaxSideSizeHalf, kMaxSideSize, 0));
            spriteShapeController.spline.InsertPointAt(5, new Vector3(kMaxSideSize, kMaxSideSizeHalf, 0));
            spriteShapeController.spline.InsertPointAt(6, new Vector3(kMaxSideSize, -kMaxSideSizeHalf, 0));
            spriteShapeController.spline.InsertPointAt(7, new Vector3(kMaxSideSizeHalf, -kMaxSideSize, 0));
        }

        public static int GetRangeIndexFromAngle(UnityEngine.U2D.SpriteShape spriteShape, float angle)
        {
            return GetRangeIndexFromAngle(spriteShape.angleRanges, angle);
        }

        public static int GetRangeIndexFromAngle(List<AngleRange> angleRanges, float angle)
        {
            for (int i = 0; i < angleRanges.Count; ++i)
            {
                var angleRange = angleRanges[i];
                var range = angleRange.end - angleRange.start;
                var angle2 = Mathf.Repeat(angle - angleRange.start, 360f);

                if (angle2 >= 0f && angle2 <= range)
                    return i;
            }

            return -1;
        }
    }
}
