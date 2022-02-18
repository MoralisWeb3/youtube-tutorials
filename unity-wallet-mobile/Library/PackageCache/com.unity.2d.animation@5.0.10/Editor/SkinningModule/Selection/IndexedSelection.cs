using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    [Serializable]
    internal class IndexedSelection : SerializableSelection<int>
    {
        protected override int GetInvalidElement() { return -1; }
    }
}
