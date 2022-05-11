using UnityEngine;
using UnityEditor;

namespace UnityEditor.U2D.Path
{
    public interface ISnapping<T>
    {
        T Snap(T value);
    }
}
