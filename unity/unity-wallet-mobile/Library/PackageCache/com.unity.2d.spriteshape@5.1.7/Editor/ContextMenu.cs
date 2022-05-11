using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D.SpriteShape;

namespace UnityEditor.U2D
{
    public class ContextMenu
    {
        private static AngleRange CreateAngleRange(float start, float end, int order)
        {
            AngleRange angleRange = new AngleRange();

            angleRange.start = start;
            angleRange.end = end;
            angleRange.order = order;

            return angleRange;
        }

        //[MenuItem("Assets/Create/Sprite Shape Profile/Open Shape", false, 358)]
        public static void CreateNewSpriteStrip()
        {
            UnityEngine.U2D.SpriteShape newSpriteShape = SpriteShapeEditorUtility.CreateSpriteShapeAsset();
            newSpriteShape.angleRanges.Add(CreateAngleRange(-180.0f, 180.0f, 0));
        }

        //[MenuItem("Assets/Create/Sprite Shape Profile/Closed Shape", false, 359)]
        public static void CreateNewSpriteShape()
        {
            UnityEngine.U2D.SpriteShape newSpriteShape = SpriteShapeEditorUtility.CreateSpriteShapeAsset();
            newSpriteShape.angleRanges.Add(CreateAngleRange(-45.0f, 45.0f, 4));
            newSpriteShape.angleRanges.Add(CreateAngleRange(-135.0f, -45.0f, 3));
            newSpriteShape.angleRanges.Add(CreateAngleRange(135.0f, 225.0f, 2));
            newSpriteShape.angleRanges.Add(CreateAngleRange(45.0f, 135.0f, 1));
        }

        //[MenuItem("GameObject/2D Object/Sprite Shape")]
        internal static void CreateSpriteShapeEmpty()
        {
            SpriteShapeEditorUtility.SetShapeFromAsset(SpriteShapeEditorUtility.CreateSpriteShapeControllerFromSelection());
        }
    }
}
