using UnityEngine;
using UnityEditor;

namespace UnityEditor.U2D.Path
{
    public class PointRectSelector : RectSelector<Vector3>
    {
        protected override bool Select(Vector3 element)
        {
            return guiRect.Contains(HandleUtility.WorldToGUIPoint(element), true);
        }
    }
}
