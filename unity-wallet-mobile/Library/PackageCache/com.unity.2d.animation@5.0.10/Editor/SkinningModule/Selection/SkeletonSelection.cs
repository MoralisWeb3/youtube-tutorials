using System;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    [Serializable]
    internal class SkeletonSelection : IBoneSelection
    {
        [SerializeField]
        private BoneSelection m_BoneSelection = new BoneSelection();

        public int Count
        {
            get { return m_BoneSelection.Count; }
        }

        public BoneCache activeElement
        {
            get { return m_BoneSelection.activeElement; }
            set
            {
                ValidateBone(value);
                m_BoneSelection.activeElement = value;
            }
        }
        public BoneCache[] elements
        {
            get { return m_BoneSelection.elements; }
            set
            {
                foreach (var bone in value)
                    ValidateBone(bone);

                m_BoneSelection.elements = value;
            }
        }

        public BoneCache root
        {
            get { return m_BoneSelection.root; }
        }

        public BoneCache[] roots
        {
            get { return m_BoneSelection.roots; }
        }

        public void BeginSelection()
        {
            m_BoneSelection.BeginSelection();
        }

        public void Clear()
        {
            m_BoneSelection.Clear();
        }

        public bool Contains(BoneCache element)
        {
            return m_BoneSelection.Contains(element);
        }

        public void EndSelection(bool select)
        {
            m_BoneSelection.EndSelection(select);
        }

        public void Select(BoneCache element, bool select)
        {
            ValidateBone(element);
            m_BoneSelection.Select(element, select);
        }

        private void ValidateBone(BoneCache bone)
        {
            if (bone == null)
                return;

            var skinningCache = bone.skinningCache;

            if (skinningCache.hasCharacter)
            {
                if (bone.skeleton != skinningCache.character.skeleton)
                    throw new Exception("Selection Exception: bone does not belong to character skeleton");
            }
            else
            {
                var selectedSprite = skinningCache.selectedSprite;

                if (selectedSprite == null)
                    throw new Exception("Selection Exception: skeleton not selected");
                else
                {
                    var skeleton = selectedSprite.GetSkeleton();

                    if (bone.skeleton != skeleton)
                        throw new Exception("Selection Exception: bone's skeleton does not match selected skeleton");
                }
            }
        }
    }
}
