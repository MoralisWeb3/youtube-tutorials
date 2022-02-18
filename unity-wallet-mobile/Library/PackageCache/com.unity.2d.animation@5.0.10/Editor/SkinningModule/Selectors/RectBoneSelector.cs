using UnityEngine;
using System.Collections.Generic;

namespace UnityEditor.U2D.Animation
{
    internal class RectBoneSelector : IRectSelector<BoneCache>
    {
        public ISelection<BoneCache> selection { get; set; }
        public BoneCache[] bones { get; set; }
        public Rect rect { get; set; }

        public void Select()
        {
            if (bones == null)
                return;
                
            foreach (var bone in bones)
            {
                if (!bone.isVisible)
                    continue;

                Vector2 p1 = bone.position;
                Vector2 p2 = bone.endPosition;
                Vector2 point = Vector2.zero;
                if (rect.Contains(p1, true) || rect.Contains(p2, true) ||
                    MathUtility.SegmentIntersection(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMax, rect.yMin), p1, p2, ref point) ||
                    MathUtility.SegmentIntersection(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMax, rect.yMax), p1, p2, ref point) ||
                    MathUtility.SegmentIntersection(new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMin, rect.yMax), p1, p2, ref point) ||
                    MathUtility.SegmentIntersection(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMin, rect.yMin), p1, p2, ref point)
                    )
                    selection.Select(bone.ToCharacterIfNeeded(), true);
            }
        }
    }
}
