using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.U2D.Animation
{
    internal class UnityEngineUndo : IUndo
    {
        public void RecordObject(object o, string name)
        {
            var obj = o as Object;
            if (obj != null)
                Undo.RecordObject(obj, name);
        }

        public void RegisterCompleteObjectUndo(object o, string name)
        {
            var obj = o as Object;
            if (obj != null)
                Undo.RegisterCompleteObjectUndo(obj, name);
        }

        public void RegisterCompleteObjectUndo(object[] o, string name)
        {
            var obj = o as Object[];
            if (obj != null)
                Undo.RegisterCompleteObjectUndo(obj, name);
        }

        public void RegisterCreatedObjectUndo(object o, string name)
        {
            var obj = o as Object;
            if (obj != null)
                Undo.RegisterCreatedObjectUndo(obj, name);
        }

        public void DestroyObjectImmediate(object o)
        {
            var obj = o as Object;
            if (obj != null)
                Undo.DestroyObjectImmediate(obj);
        }

        public void ClearUndo(object o)
        {
            var obj = o as Object;
            if (obj != null)
                Undo.ClearUndo(obj);
        }

        public void IncrementCurrentGroup()
        {
            Undo.IncrementCurrentGroup();
        }
    }
}
