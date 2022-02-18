using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Path
{
    public enum ShapeType
    {
        Polygon,
        Spline
    }

    public interface IShape
    {
        ShapeType type { get; }
        bool isOpenEnded { get; }
        ControlPoint[] ToControlPoints();
    }
}
