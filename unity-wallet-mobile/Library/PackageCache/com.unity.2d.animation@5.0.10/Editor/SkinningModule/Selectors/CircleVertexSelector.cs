using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class CircleVertexSelector : ICircleSelector<int>
    {
        public ISelection<int> selection { get; set; }
        public ISpriteMeshData spriteMeshData { get; set; }
        public Vector2 position { get; set; }
        public float radius { get; set; }

        public void Select()
        {
            if(spriteMeshData == null)
                return;
                
            var sqrRadius = radius * radius;

            for (int i = 0; i < spriteMeshData.vertexCount; i++)
            {
                if ((spriteMeshData.GetPosition(i) - position).sqrMagnitude <= sqrRadius)
                {
                    selection.Select(i, true);
                }
            }
        }
    }
}
