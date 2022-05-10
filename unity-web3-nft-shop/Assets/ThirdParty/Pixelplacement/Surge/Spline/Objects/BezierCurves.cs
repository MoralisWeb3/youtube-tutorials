/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Methods for evaluating curves.
/// 
/// </summary>

using UnityEngine;

namespace Pixelplacement
{
    public static class BezierCurves
    {
        //Quadratic Bezier:
        public static Vector3 GetPoint (Vector3 startPosition, Vector3 controlPoint, Vector3 endPosition, float percentage)
        {
            percentage = Mathf.Clamp01 (percentage);
            float oneMinusT = 1f - percentage;
            return oneMinusT * oneMinusT * startPosition + 2f * oneMinusT * percentage * controlPoint + percentage * percentage * endPosition;
        }

        public static Vector3 GetFirstDerivative (Vector3 startPoint, Vector3 controlPoint, Vector3 endPosition, float percentage)
        {
            percentage = Mathf.Clamp01 (percentage);
            return 2f * (1f - percentage) * (controlPoint - startPoint) + 2f * percentage * (endPosition - controlPoint);
        }

        //Cubic Bezier:
        public static Vector3 GetPoint (Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, float percentage, bool evenDistribution, int distributionSteps)
        {
            if (evenDistribution)
            {
                int maxPoint = distributionSteps + 1;
                float[] arcLengths = new float[maxPoint];
                Vector3 previousPoint = Locate(startPosition, endPosition, startTangent, endTangent, 0);
                float sum = 0;

                //store arc lengths:
                for (int i = 1; i < maxPoint; i++)
                {
                    Vector3 p = Locate(startPosition, endPosition, startTangent, endTangent, i / (float)maxPoint);
                    sum += Vector3.Distance(previousPoint, p);
                    arcLengths[i] = sum;
                    previousPoint = p;
                }

                float targetLength = percentage * arcLengths[distributionSteps];

                //search:
                int low = 0;
                int high = distributionSteps;
                int index = 0;
                while (low < high)
                {
                    index = low + (((high - low) / 2) | 0);
                    if (arcLengths[index] < targetLength)
                    {
                        low = index + 1;

                    }
                    else
                    {
                        high = index;
                    }
                }

                //adjust:
                if (arcLengths[index] > targetLength)
                {
                    index--;
                }

                float lengthBefore = arcLengths[index];

                //interpolate or use as is:
                if (lengthBefore == targetLength)
                {
                    return Locate(startPosition, endPosition, startTangent, endTangent, index / distributionSteps);
                }
                else
                {
                    return Locate(startPosition, endPosition, startTangent, endTangent, (index + (targetLength - lengthBefore) / (arcLengths[index + 1] - lengthBefore)) / distributionSteps);
                }
            }

            return Locate(startPosition, endPosition, startTangent, endTangent, percentage);
        }

        public static Vector3 GetFirstDerivative (Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, float percentage)
        {
            percentage = Mathf.Clamp01 (percentage);
            float oneMinusT = 1f - percentage;
            return 3f * oneMinusT * oneMinusT * (startTangent - startPosition) + 6f * oneMinusT * percentage * (endTangent - startTangent) + 3f * percentage * percentage * (endPosition - endTangent);
        }

        private static Vector3 Locate(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, float percentage)
        {
            percentage = Mathf.Clamp01(percentage);
            float oneMinusT = 1f - percentage;
            return oneMinusT * oneMinusT * oneMinusT * startPosition + 3f * oneMinusT * oneMinusT * percentage * startTangent + 3f * oneMinusT * percentage * percentage * endTangent + percentage * percentage * percentage * endPosition;
        }
    }
}