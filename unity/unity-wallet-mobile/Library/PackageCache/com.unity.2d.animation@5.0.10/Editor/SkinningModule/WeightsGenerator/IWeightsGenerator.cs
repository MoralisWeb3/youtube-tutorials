using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal interface IWeightsGenerator
    {
        BoneWeight[] Calculate(Vector2[] vertices, Edge[] edges, Vector2[] controlPoints, Edge[] bones, int[] pins);
    }
}
