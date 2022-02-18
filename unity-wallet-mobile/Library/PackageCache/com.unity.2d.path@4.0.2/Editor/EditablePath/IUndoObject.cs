using UnityEngine;

namespace UnityEditor.U2D.Path
{
    public interface IUndoObject
    {
        void RegisterUndo(string name);
    }
}
