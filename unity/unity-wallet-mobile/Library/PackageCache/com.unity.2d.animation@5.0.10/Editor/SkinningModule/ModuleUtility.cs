using UnityEngine;
using System;
using System.Collections.Generic;


namespace UnityEditor.U2D.Animation
{
    internal static class ModuleUtility
    {
        public static Vector3 GUIToWorld(Vector3 guiPosition)
        {
            return GUIToWorld(guiPosition, Vector3.forward, Vector3.zero);
        }

        public static Vector3 GUIToWorld(Vector3 guiPosition, Vector3 planeNormal, Vector3 planePos)
        {
            Vector3 worldPos = Handles.inverseMatrix.MultiplyPoint(guiPosition);

            if (Camera.current)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(guiPosition);

                planeNormal = Handles.matrix.MultiplyVector(planeNormal);

                planePos = Handles.matrix.MultiplyPoint(planePos);

                Plane plane = new Plane(planeNormal, planePos);

                float distance = 0f;

                if (plane.Raycast(ray, out distance))
                {
                    worldPos = Handles.inverseMatrix.MultiplyPoint(ray.GetPoint(distance));
                }
            }

            return worldPos;
        }

        public static GUIContent[] ToGUIContentArray(string[] names)
        {
            return Array.ConvertAll(names, n => new GUIContent(n));
        }

        public static Color CalculateNiceColor(int index, int numColors)
        {
            numColors = Mathf.Clamp(numColors, 1, int.MaxValue);

            int loops = index / numColors;
            index = index % 360;

            int hueAngleStep = 360 / numColors;
            float hueLoopOffset = hueAngleStep * 0.5f;
            float hue = index * hueAngleStep + loops * hueLoopOffset;

            return Color.HSVToRGB(Mathf.Repeat(hue, 360f) / 360f, 1f, 1f);
        }

        public static void UpdateLocalToWorldMatrices(List<SpriteBoneData> spriteBoneDataList, Matrix4x4 rootMatrix, ref Matrix4x4[] localToWorldMatrices)
        {
            if (localToWorldMatrices == null || localToWorldMatrices.Length != spriteBoneDataList.Count)
                localToWorldMatrices = new Matrix4x4[spriteBoneDataList.Count];

            bool[] calculatedMatrix = new bool[spriteBoneDataList.Count];

            var processedBoneCount = 0;
            while (processedBoneCount < spriteBoneDataList.Count)
            {
                int oldCount = processedBoneCount;

                for (var i = 0; i < spriteBoneDataList.Count; ++i)
                {
                    if (calculatedMatrix[i])
                        continue;

                    var sourceBone = spriteBoneDataList[i];
                    if (sourceBone.parentId != -1 && !calculatedMatrix[sourceBone.parentId])
                        continue;

                    var localToWorldMatrix = Matrix4x4.identity;
                    localToWorldMatrix.SetTRS(sourceBone.localPosition, sourceBone.localRotation, Vector3.one);

                    if (sourceBone.parentId == -1)
                        localToWorldMatrix = rootMatrix * localToWorldMatrix;
                    else if (calculatedMatrix[sourceBone.parentId])
                        localToWorldMatrix = localToWorldMatrices[sourceBone.parentId] * localToWorldMatrix;

                    localToWorldMatrices[i] = localToWorldMatrix;
                    calculatedMatrix[i] = true;
                    processedBoneCount++;
                }

                if (oldCount == processedBoneCount)
                    throw new ArgumentException("Invalid hierarchy detected");
            }
        }

        public static List<SpriteBoneData> CreateSpriteBoneData(UnityEngine.U2D.SpriteBone[] spriteBoneList, Matrix4x4 rootMatrix)
        {
            List<SpriteBoneData> spriteBoneDataList = new List<SpriteBoneData>(spriteBoneList.Length);

            foreach (var spriteBone in spriteBoneList)
            {
                spriteBoneDataList.Add(new SpriteBoneData()
                {
                    name = spriteBone.name,
                    parentId = spriteBone.parentId,
                    localPosition = spriteBone.position,
                    localRotation = spriteBone.rotation,
                    depth = spriteBone.position.z,
                    length = spriteBone.length
                });
            }

            Matrix4x4[] localToWorldMatrices = null;
            UpdateLocalToWorldMatrices(spriteBoneDataList, rootMatrix, ref localToWorldMatrices);

            for (int i = 0; i < spriteBoneDataList.Count; ++i)
            {
                var spriteBoneData = spriteBoneDataList[i];
                spriteBoneData.position = localToWorldMatrices[i].MultiplyPoint(Vector2.zero);
                spriteBoneData.endPosition = localToWorldMatrices[i].MultiplyPoint(Vector2.right * spriteBoneData.length);
            }

            return spriteBoneDataList;
        }
    }
}
