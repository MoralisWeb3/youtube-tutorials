using UnityEngine;
using UnityHandles = UnityEditor.Handles;
using UnityTexture2D = UnityEngine.Texture2D;

namespace UnityEditor.U2D.Sprites
{
    internal interface IHandles
    {
        Color color { get; set; }
        Matrix4x4 matrix { get; set; }

        Vector3[] MakeBezierPoints(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int division);

        void DrawAAPolyLine(ITexture2D lineTex, float width, params Vector3[] points);
        void DrawAAPolyLine(ITexture2D lineTex, params Vector3[] points);

        void DrawLine(Vector3 p1, Vector3 p2);

        void SetDiscSectionPoints(Vector3[] dest, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius);
    }

    internal class HandlesSystem : IHandles
    {
        static IHandles m_System;

        static public void SetSystem(IHandles system)
        {
            m_System = system;
        }

        static public IHandles GetSystem()
        {
            if (m_System == null)
                m_System = new HandlesSystem();
            return m_System;
        }

        public Color color
        {
            get { return UnityHandles.color; }
            set { UnityHandles.color = value; }
        }
        public Matrix4x4 matrix
        {
            get { return UnityHandles.matrix; }
            set { UnityHandles.matrix = value; }
        }

        public Vector3[] MakeBezierPoints(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, int division)
        {
            return UnityHandles.MakeBezierPoints(startPosition, endPosition, startTangent, endTangent, division);
        }

        public void DrawAAPolyLine(ITexture2D lineTex, float width, params Vector3[] points)
        {
            UnityHandles.DrawAAPolyLine((UnityTexture2D)lineTex, width, points);
        }

        public void DrawAAPolyLine(ITexture2D lineTex, params Vector3[] points)
        {
            UnityHandles.DrawAAPolyLine((UnityTexture2D)lineTex, points);
        }

        public void DrawLine(Vector3 p1, Vector3 p2)
        {
            UnityHandles.DrawLine(p1, p2);
        }

        public void SetDiscSectionPoints(Vector3[] dest, Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
        {
            UnityHandles.SetDiscSectionPoints(dest, center, normal, from, angle, radius);
        }
    }
}
