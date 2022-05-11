/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Used for easily attaching objects to a spline for inspector usage.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

namespace Pixelplacement
{
    [System.Serializable]
    public class SplineFollower
    {
        //Public Variables:
        public Transform target;
        public float percentage = -1;
        public bool faceDirection;

        //Public Properties:
        public bool WasMoved
        {
            get
            {
                if (percentage != _previousPercentage || faceDirection != _previousFaceDirection) {
                    _previousPercentage = percentage;
                    _previousFaceDirection = faceDirection;
                    return true;
                } else {
                    return false;	
                }
            }
        }

        //Private Variables:
        float _previousPercentage;
        bool _previousFaceDirection;
        bool _detached;

        //Public Methods:
        public void UpdateOrientation (Spline spline)
        {
            if (target == null) return;

            //clamp percentage:
            if (!spline.loop) percentage = Mathf.Clamp01 (percentage);

            //look in direction of spline?
            if (faceDirection)
            {
                if (spline.direction == SplineDirection.Forward)
                {
                    target.LookAt (target.position + spline.GetDirection (percentage));
                }else{
                    target.LookAt (target.position - spline.GetDirection (percentage));
                }
            }

            target.position = spline.GetPosition (percentage);
        }
    }
}