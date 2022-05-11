using System.Collections.Generic;

namespace UnityEditor.U2D.Path
{
    public interface ISelection<T>
    {
        int Count { get; }
        T activeElement { get; set; }
        T[] elements { get; set; }
        void Clear();
        void BeginSelection();
        void EndSelection(bool select);
        bool Select(T element, bool select);
        bool Contains(T element);
    }
}
