using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal enum SkinningMode
    {
        SpriteSheet,
        Character
    }

    internal enum Tools
    {
        EditGeometry,
        CreateVertex,
        CreateEdge,
        SplitEdge,
        GenerateGeometry,
        EditPose,
        EditJoints,
        CreateBone,
        SplitBone,
        ReparentBone,
        WeightSlider,
        WeightBrush,
        GenerateWeights,
        BoneInfluence,
        CopyPaste,
        Visibility,
        SwitchMode
    }
}
