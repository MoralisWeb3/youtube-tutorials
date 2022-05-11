namespace UnityEditor.U2D.Path
{
    public interface ISelector<T>
    {
        bool Select(T element);
    }
}
