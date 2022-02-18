using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class DisabledUndo : IUndo
    {
        public void RecordObject(object o, string name) {}
        public void RegisterCompleteObjectUndo(object o, string name) {}
        public void RegisterCompleteObjectUndo(object[] o, string name) {}
        public void RegisterCreatedObjectUndo(object o, string name) {}
        public void DestroyObjectImmediate(object o)
        {
            BaseObject.DestroyImmediate(o);
        }

        public void ClearUndo(object o) {}
        public void IncrementCurrentGroup() {}
    }
}
