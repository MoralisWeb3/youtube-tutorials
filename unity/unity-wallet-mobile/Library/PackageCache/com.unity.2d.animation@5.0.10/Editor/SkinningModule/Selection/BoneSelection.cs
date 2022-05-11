using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    [Serializable]
    internal class BoneSelection : SerializableSelection<BoneCache>, IBoneSelection
    {
        protected override BoneCache GetInvalidElement() { return null; }

        public BoneCache root
        {
            get { return activeElement.FindRoot<BoneCache>(elements); }
        }

        public BoneCache[] roots
        {
            get { return elements.FindRoots<BoneCache>(); }
        }
    }
}
