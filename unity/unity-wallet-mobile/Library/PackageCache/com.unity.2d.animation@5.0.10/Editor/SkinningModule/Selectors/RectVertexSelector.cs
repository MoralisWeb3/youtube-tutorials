using UnityEngine;
using System.Collections.Generic;

namespace UnityEditor.U2D.Animation
{
    internal class RectVertexSelector : IRectSelector<int>
    {
        public ISelection<int> selection { get; set; }
        public ISpriteMeshData spriteMeshData { get; set; }
        public Rect rect { get; set; }

        public void Select()
        {
            for (int i = 0; i < spriteMeshData.vertexCount; i++)
            {
                if (rect.Contains(spriteMeshData.GetPosition(i), true))
                    selection.Select(i, true);
            }
        }
    }
}
