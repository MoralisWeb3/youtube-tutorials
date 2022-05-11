using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class SmoothingUtility
    {
        private static float[,] m_DataInTemp;
        private static float[,] m_DataOutTemp;
        private static float[] m_DenominatorTemp;
        private static EditableBoneWeight s_BoneWeight = new EditableBoneWeight();

        public static void SmoothWeights(BoneWeight[] boneWeightIn, IList<int> indices, int boneCount, out BoneWeight[] boneWeightOut)
        {
            SmoothWeights(boneWeightIn, indices, boneCount, 1, out boneWeightOut);
        }

        public static void SmoothWeights(BoneWeight[] boneWeightIn, IList<int> indices, int boneCount, int iterations, out BoneWeight[] boneWeightOut)
        {
            Debug.Assert(boneWeightIn != null);

            boneWeightOut = new BoneWeight[boneWeightIn.Length];

            PrepareTempBuffers(boneWeightIn.Length, boneCount);

            for (int i = 0; i < boneWeightIn.Length; ++i)
            {
                s_BoneWeight.SetFromBoneWeight(boneWeightIn[i]);
                for (var j = 0; j < s_BoneWeight.Count; ++j)
                {
                    if (s_BoneWeight[j].enabled)
                        m_DataInTemp[i, s_BoneWeight[j].boneIndex] = s_BoneWeight[j].weight;
                }
            }

            for (var i = 0; i < iterations; ++i)
                SmoothPerVertexData(indices, m_DataInTemp, m_DataOutTemp);

            for (var i = 0; i < boneWeightIn.Length; ++i)
            {
                s_BoneWeight.Clear();

                for (var j = 0; j < boneCount; ++j)
                {
                    var weight = m_DataOutTemp[i, j];
                    var boneIndex = weight > 0f ? j : 0;
                    s_BoneWeight.AddChannel(boneIndex, weight, weight > 0);
                }

                s_BoneWeight.Clamp(4);
                s_BoneWeight.Normalize();

                boneWeightOut[i] = s_BoneWeight.ToBoneWeight(false);
            }
        }

        public static void SmoothPerVertexData(IList<int> indices, float[,] dataIn, float[,] dataOut)
        {
            Debug.Assert(dataIn != null);
            Debug.Assert(dataOut != null);
            Debug.Assert(dataIn != dataOut);
            Debug.Assert(dataIn.Length == dataOut.Length);

            int rowLength = dataIn.GetLength(0);
            int colLength = dataIn.GetLength(1);

            PrepareDenominatorBuffer(rowLength);

            for (int i = 0; i < indices.Count / 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    int j1 = (j + 1) % 3;
                    int j2 = (j + 2) % 3;

                    for (int k = 0; k < colLength; ++k)
                        dataOut[indices[i * 3 + j], k] += dataIn[indices[i * 3 + j1], k] + dataIn[indices[i * 3 + j2], k];

                    m_DenominatorTemp[indices[i * 3 + j]] += 2;
                }
            }

            for (int i = 0; i < rowLength; ++i)
            {
                var dInv = 1f / Mathf.Max(1f, m_DenominatorTemp[i]);
                for (int j = 0; j < colLength; ++j)
                    dataOut[i, j] *= dInv;
            }
        }

        private static void PrepareDenominatorBuffer(int rowLength)
        {
            if (m_DenominatorTemp == null || m_DenominatorTemp.Length != rowLength)
                m_DenominatorTemp = new float[rowLength];
            else
                Array.Clear(m_DenominatorTemp, 0, m_DenominatorTemp.Length);
        }

        private static void PrepareTempBuffers(int rowLength, int colLength)
        {
            if (m_DataInTemp == null || m_DataInTemp.GetLength(0) != rowLength || m_DataInTemp.GetLength(1) != colLength)
                m_DataInTemp = new float[rowLength, colLength];
            else
                Array.Clear(m_DataInTemp, 0, m_DataInTemp.Length);

            if (m_DataOutTemp == null || m_DataOutTemp.GetLength(0) != rowLength || m_DataOutTemp.GetLength(1) != colLength)
                m_DataOutTemp = new float[rowLength, colLength];
            else
                Array.Clear(m_DataOutTemp, 0, m_DataOutTemp.Length);
        }
    }
}
