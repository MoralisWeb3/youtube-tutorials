using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class TransformCache : SkinningObject, IEnumerable<TransformCache>
    {
        [SerializeField]
        private TransformCache m_Parent;
        [SerializeField]
        private List<TransformCache> m_Children = new List<TransformCache>();
        [SerializeField]
        private Vector3 m_LocalPosition;
        [SerializeField]
        private Quaternion m_LocalRotation = Quaternion.identity;
        [SerializeField]
        private Vector3 m_LocalScale = Vector3.one;
        [SerializeField]
        private Matrix4x4 m_LocalToWorldMatrix = Matrix4x4.identity;

        public TransformCache parent
        {
            get { return m_Parent; }
        }

        public TransformCache[] children
        {
            get { return m_Children.ToArray(); }
        }

        internal virtual int siblingIndex
        {
            get { return GetSiblingIndex(); }
            set { SetSiblingIndex(value); }
        }

        public int ChildCount
        {
            get { return m_Children.Count; }
        }

        public Vector3 localPosition
        {
            get { return m_LocalPosition; }
            set
            {
                m_LocalPosition = value;
                Update();
            }
        }

        public Quaternion localRotation
        {
            get { return m_LocalRotation; }
            set
            {
                m_LocalRotation = MathUtility.NormalizeQuaternion(value);
                Update();
            }
        }

        public Vector3 localScale
        {
            get { return m_LocalScale; }
            set
            {
                m_LocalScale = value;
                Update();
            }
        }

        public Vector3 position
        {
            get { return parentMatrix.MultiplyPoint3x4(localPosition); }
            set { localPosition = parentMatrix.inverse.MultiplyPoint3x4(value); }
        }

        public Quaternion rotation
        {
            get { return GetGlobalRotation(); }
            set { SetGlobalRotation(value); }
        }

        public Vector3 right
        {
            get { return localToWorldMatrix.MultiplyVector(Vector3.right).normalized; }
            set { MatchDirection(Vector3.right, value); }
        }

        public Vector3 up
        {
            get { return localToWorldMatrix.MultiplyVector(Vector3.up).normalized; }
            set { MatchDirection(Vector3.up, value); }
        }

        public Vector3 forward
        {
            get { return localToWorldMatrix.MultiplyVector(Vector3.forward).normalized; }
            set { MatchDirection(Vector3.forward, value); }
        }

        public Matrix4x4 localToWorldMatrix
        {
            get { return m_LocalToWorldMatrix; }
        }

        public Matrix4x4 worldToLocalMatrix
        {
            get { return localToWorldMatrix.inverse; }
        }

        private Matrix4x4 parentMatrix
        {
            get
            {
                var parentMatrix = Matrix4x4.identity;

                if (parent != null)
                    parentMatrix = parent.localToWorldMatrix;

                return parentMatrix;
            }
        }

        internal override void OnDestroy()
        {
            if (parent != null)
                parent.RemoveChild(this);

            m_Parent = null;
            m_Children.Clear();
        }

        private void Update()
        {
            m_LocalToWorldMatrix = parentMatrix * Matrix4x4.TRS(localPosition, localRotation, localScale);

            foreach (var child in m_Children)
                child.Update();
        }

        private void AddChild(TransformCache transform)
        {
            m_Children.Add(transform);
        }

        private void InsertChildAt(int index, TransformCache transform)
        {
            m_Children.Insert(index, transform);
        }

        private void RemoveChild(TransformCache transform)
        {
            m_Children.Remove(transform);
        }

        private void RemoveChildAt(int index)
        {
            m_Children.RemoveAt(index);
        }

        private int GetSiblingIndex()
        {
            if (parent == null)
                return -1;

            return parent.m_Children.IndexOf(this);
        }
        private void SetSiblingIndex(int index)
        {
            if (parent != null)
            {
                var currentIndex = parent.m_Children.IndexOf(this);
                var indexToRemove = index < currentIndex ? currentIndex + 1 : currentIndex;
                parent.InsertChildAt(index, this);
                parent.RemoveChildAt(indexToRemove);
            }
        }

        public void SetParent(TransformCache newParent)
        {
            SetParent(newParent, true);
        }

        public void SetParent(TransformCache newParent, bool worldPositionStays)
        {
            if (m_Parent == newParent)
                return;

            var oldPosition = position;
            var oldRotation = rotation;

            if (m_Parent != null)
                m_Parent.RemoveChild(this);

            m_Parent = newParent;

            if (m_Parent != null)
                m_Parent.AddChild(this);

            if (worldPositionStays)
            {
                position = oldPosition;
                rotation = oldRotation;
            }
            else
            {
                Update();
            }
        }

        private Quaternion GetGlobalRotation()
        {
            var globalRotation = localRotation;
            var currentParent = parent;

            while (currentParent != null)
            {
                globalRotation = ScaleMulQuat(currentParent.localScale, globalRotation);
                globalRotation = currentParent.localRotation * globalRotation;
                currentParent = currentParent.parent;
            }

            return globalRotation;
        }

        private void SetGlobalRotation(Quaternion r)
        {
            if (parent != null)
                r = parent.InverseTransformRotation(r);
            localRotation = r;
        }

        private Quaternion InverseTransformRotation(Quaternion r)
        {
            if (parent != null)
                r = parent.InverseTransformRotation(r);

            r = Quaternion.Inverse(localRotation) * r;
            r = ScaleMulQuat(localScale, r);

            return r;
        }

        private Quaternion ScaleMulQuat(Vector3 scale, Quaternion q)
        {
            var s = new Vector3(Chgsign(1f, scale.x), Chgsign(1f, scale.y), Chgsign(1f, scale.z));
            q.x = Chgsign(q.x, s.y * s.z);
            q.y = Chgsign(q.y, s.x * s.z);
            q.z = Chgsign(q.z, s.x * s.y);
            return q;
        }

        private float Chgsign(float x, float y)
        {
            return y < 0f ? -x : x;
        }

        private void MatchDirection(Vector3 localDirection, Vector3 worldDirection)
        {
            var direction = worldToLocalMatrix.MultiplyVector(worldDirection);
            direction = Matrix4x4.TRS(Vector3.zero, localRotation, localScale).MultiplyVector(direction);
            var scaledLocalDirection = Vector3.Scale(localDirection, localScale);
            var deltaRotation = Quaternion.identity;

            if (scaledLocalDirection.sqrMagnitude > 0f)
            {
                var axis = Vector3.Cross(scaledLocalDirection, direction);
                var angle = Vector3.SignedAngle(scaledLocalDirection, direction, axis);
                deltaRotation = Quaternion.AngleAxis(angle, axis);
            }

            localRotation = deltaRotation;
        }

        IEnumerator<TransformCache> IEnumerable<TransformCache>.GetEnumerator()
        {
            return m_Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)m_Children.GetEnumerator();
        }
    }
}
