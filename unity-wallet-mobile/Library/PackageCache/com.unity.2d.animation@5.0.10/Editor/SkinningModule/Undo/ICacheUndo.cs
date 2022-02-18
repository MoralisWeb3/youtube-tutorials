namespace UnityEditor.U2D.Animation
{
    internal interface ICacheUndo
    {
        IUndo undoOverride { get; set; }
        bool isUndoOperationSet { get; }
        void IncrementCurrentGroup();
        void BeginUndoOperation(string name);
        void EndUndoOperation();
    }
}
