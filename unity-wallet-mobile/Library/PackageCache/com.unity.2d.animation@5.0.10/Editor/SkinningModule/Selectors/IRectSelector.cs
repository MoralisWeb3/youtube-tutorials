using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal interface IRectSelector<T> : ISelector<T>
    {
        Rect rect { get; set; }
    }
}
