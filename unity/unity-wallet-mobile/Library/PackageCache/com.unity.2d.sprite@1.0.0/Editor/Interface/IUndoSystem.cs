using UnityEngine;

namespace UnityEditor.U2D.Sprites
{
    internal interface IUndoSystem
    {
        void RegisterUndoCallback(Undo.UndoRedoCallback undoCallback);
        void UnregisterUndoCallback(Undo.UndoRedoCallback undoCallback);
        void RegisterCompleteObjectUndo(ScriptableObject obj, string undoText);
        void ClearUndo(ScriptableObject obj);
    }

    internal class UndoSystem : IUndoSystem
    {
        public void RegisterUndoCallback(Undo.UndoRedoCallback undoCallback)
        {
            Undo.undoRedoPerformed += undoCallback;
        }

        public void UnregisterUndoCallback(Undo.UndoRedoCallback undoCallback)
        {
            Undo.undoRedoPerformed -= undoCallback;
        }

        public void RegisterCompleteObjectUndo(ScriptableObject so, string undoText)
        {
            if (so != null)
            {
                Undo.RegisterCompleteObjectUndo(so, undoText);
            }
        }

        public void ClearUndo(ScriptableObject so)
        {
            if (so != null)
            {
                Undo.ClearUndo(so);
            }
        }
    }
}
