using UnityEngine;
using UnityEditor;

namespace UnityEditor.U2D.Path
{
    public class EditablePathUtility
    {
        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}
