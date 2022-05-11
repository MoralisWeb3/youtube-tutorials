namespace UnityEditor.U2D.Animation
{
    internal interface IUndo
    {
        void RecordObject(object o, string name);
        void RegisterCompleteObjectUndo(object o, string name);
        void RegisterCompleteObjectUndo(object[] o, string name);
        void RegisterCreatedObjectUndo(object o, string name);
        void DestroyObjectImmediate(object o);
        void ClearUndo(object o);
        void IncrementCurrentGroup();
    }
}
