using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal interface ITransformSelection<T> : ISelection<T> where T : TransformCache
    {
        T root { get; }
        T[] roots { get; }
    }
}
