using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// Dynamic modification of spline to follow the path of mouse movement.
// This script is just a simplified demo to demonstrate the idea.
namespace SpriteShapeExtras
{

    public class SimpleDraw : MonoBehaviour
    {
        public float minimumDistance = 1.0f;
        private Vector3 lastPosition;
    
        // Use this for initialization
        void Start()
        {
    
        }
    
        private static int NextIndex(int index, int pointCount)
        {
            return Mod(index + 1, pointCount);
        }
    
        private static int PreviousIndex(int index, int pointCount)
        {
            return Mod(index - 1, pointCount);
        }
    
        private static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    
        private void Smoothen(SpriteShapeController sc, int pointIndex)
        {
            Vector3 position = sc.spline.GetPosition(pointIndex);
            Vector3 positionNext = sc.spline.GetPosition(NextIndex(pointIndex, sc.spline.GetPointCount()));
            Vector3 positionPrev = sc.spline.GetPosition(PreviousIndex(pointIndex, sc.spline.GetPointCount()));
            Vector3 forward = gameObject.transform.forward;
    
            float scale = Mathf.Min((positionNext - position).magnitude, (positionPrev - position).magnitude) * 0.33f;
    
            Vector3 leftTangent = (positionPrev - position).normalized * scale;
            Vector3 rightTangent = (positionNext - position).normalized * scale;
    
            sc.spline.SetTangentMode(pointIndex, ShapeTangentMode.Continuous);
            SplineUtility.CalculateTangents(position, positionPrev, positionNext, forward, scale, out rightTangent, out leftTangent);
    
            sc.spline.SetLeftTangent(pointIndex, leftTangent);
            sc.spline.SetRightTangent(pointIndex, rightTangent);
        }
    
        // Update is called once per frame
        void Update()
        {
            var mp = Input.mousePosition;
            mp.z = 10.0f;
            mp = Camera.main.ScreenToWorldPoint(mp);
            var dt = Mathf.Abs((mp - lastPosition).magnitude);
            var md = (minimumDistance > 1.0f) ? minimumDistance : 1.0f;
            if (Input.GetMouseButton(0) && dt > md)
            {
                var spriteShapeController = gameObject.GetComponent<SpriteShapeController>();
                var spline = spriteShapeController.spline;
                spline.InsertPointAt(spline.GetPointCount(), mp);
                var newPointIndex = spline.GetPointCount() - 1;
                Smoothen(spriteShapeController, newPointIndex - 1);
    
                spline.SetHeight(newPointIndex, UnityEngine.Random.Range(0.1f, 2.0f));
                lastPosition = mp;
            }
        }
    }
}