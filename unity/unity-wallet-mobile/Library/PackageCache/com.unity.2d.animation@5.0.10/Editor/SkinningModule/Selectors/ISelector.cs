using System.Collections.Generic;

namespace UnityEditor.U2D.Animation
{
    internal interface ISelector<T>
    {
        ISelection<T> selection { get; set; }

        void Select();
    }
}
