using UnityEngine;
using UnityEditor;

namespace UnityEditor.U2D.Path
{
    public class Snapping : ISnapping<Vector3>
    {
        public Vector3 Snap(Vector3 position)
        {
            return new Vector3(
                Snap(position.x, EditorSnapSettings.move.x),
                Snap(position.y, EditorSnapSettings.move.y),
                position.z);
        }

        private float Snap(float value, float snap)
        {
            return Mathf.Round(value / snap) * snap;
        }
    }
}
