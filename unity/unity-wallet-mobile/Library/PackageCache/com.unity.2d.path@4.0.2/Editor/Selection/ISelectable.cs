namespace UnityEditor.U2D.Path
{
    public interface ISelectable<T>
    {
        bool Select(ISelector<T> selector);
    }
}
