using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal enum WeightEditorMode
    {
        AddAndSubtract,
        GrowAndShrink,
        Smooth
    }

    internal class WeightEditor
    {
        public ISpriteMeshData spriteMeshData
        {
            get { return m_SpriteMeshDataController.spriteMeshData; }
            set { m_SpriteMeshDataController.spriteMeshData = value; }
        }

        public ICacheUndo cacheUndo { get; set; }
        public WeightEditorMode mode { get; set; }
        public int boneIndex { get; set; }
        public ISelection<int> selection { get; set; }
        public WeightEditorMode currentMode { get; private set; }
        public bool useRelativeValues { get; private set; }
        public bool emptySelectionEditsAll { get; set; }
        public bool autoNormalize { get; set; }

        private SpriteMeshDataController m_SpriteMeshDataController = new SpriteMeshDataController();
        private const int maxSmoothIterations = 8;
        private float[] m_SmoothValues;
        private readonly List<BoneWeight[]> m_SmoothedBoneWeights = new List<BoneWeight[]>();
        private readonly List<BoneWeight> m_StoredBoneWeights = new List<BoneWeight>();
        private int BoneCount
        {
            get { return spriteMeshData != null ? spriteMeshData.boneCount : 0; }
        }

        public WeightEditor()
        {
            autoNormalize = true;
        }

        public void OnEditStart(bool relative)
        {
            Validate();
            
            RegisterUndo();
            currentMode = mode;
            useRelativeValues = relative;

            if (!useRelativeValues)
                StoreBoneWeights();

            if (mode == WeightEditorMode.Smooth)
                PrepareSmoothingBuffers();
        }

        public void OnEditEnd()
        {
            Validate();

            if (currentMode == WeightEditorMode.AddAndSubtract)
            {
                for (int i = 0; i < spriteMeshData.vertexCount; ++i)
                    spriteMeshData.GetWeight(i).Clamp(4);
            }

            if (autoNormalize)
                m_SpriteMeshDataController.NormalizeWeights(null);

            m_SpriteMeshDataController.SortTrianglesByDepth();
        }

        public void DoEdit(float value)
        {
            Validate();

            if (!useRelativeValues)
                RestoreBoneWeights();

            if (currentMode == WeightEditorMode.AddAndSubtract)
                SetWeight(value);
            else if (currentMode == WeightEditorMode.GrowAndShrink)
                SetWeight(value, false);
            else if (currentMode == WeightEditorMode.Smooth)
                SmoothWeights(value);
        }

        private void Validate()
        {
            if (spriteMeshData == null)
                throw (new Exception(TextContent.noSpriteSelected));
        }

        private void RegisterUndo()
        {
            Debug.Assert(cacheUndo != null);

            cacheUndo.BeginUndoOperation(TextContent.editWeights);
        }

        private void SetWeight(float value, bool createNewChannel = true)
        {
            if (boneIndex == -1 || spriteMeshData == null)
                return;

            Debug.Assert(selection != null);

            for (var i = 0; i < spriteMeshData.vertexCount; ++i)
            {
                if (selection.Count == 0 && emptySelectionEditsAll ||
                    selection.Count > 0 && selection.Contains(i))
                {
                    var editableBoneWeight = spriteMeshData.GetWeight(i);

                    int channel = editableBoneWeight.GetChannelFromBoneIndex(boneIndex);

                    if (channel == -1)
                    {
                        if (createNewChannel && value > 0f)
                        {
                            editableBoneWeight.AddChannel(boneIndex, 0f, true);
                            channel = editableBoneWeight.GetChannelFromBoneIndex(boneIndex);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    editableBoneWeight[channel].weight += value;

                    if (editableBoneWeight.Sum() > 1f)
                        editableBoneWeight.CompensateOtherChannels(channel);

                    editableBoneWeight.FilterChannels(0f);
                }
            }
        }

        private void SmoothWeights(float value)
        {
            Debug.Assert(selection != null);

            for (int i = 0; i < spriteMeshData.vertexCount; ++i)
            {
                if (selection.Count == 0 && emptySelectionEditsAll ||
                    selection.Count > 0 && selection.Contains(i))
                {
                    var smoothValue = m_SmoothValues[i];

                    if (smoothValue >= maxSmoothIterations)
                        continue;

                    m_SmoothValues[i] = Mathf.Clamp(smoothValue + value, 0f, maxSmoothIterations);

                    float lerpValue = GetLerpValue(m_SmoothValues[i]);
                    int lerpIndex = GetLerpIndex(m_SmoothValues[i]);
                    BoneWeight[] smoothedBoneWeightsFloor = GetSmoothedBoneWeights(lerpIndex - 1);
                    BoneWeight[] smoothedBoneWeightsCeil = GetSmoothedBoneWeights(lerpIndex);

                    BoneWeight boneWeight = EditableBoneWeightUtility.Lerp(smoothedBoneWeightsFloor[i], smoothedBoneWeightsCeil[i], lerpValue);
                    spriteMeshData.GetWeight(i).SetFromBoneWeight(boneWeight);
                }
            }
        }

        protected void PrepareSmoothingBuffers()
        {
            if (m_SmoothValues == null || m_SmoothValues.Length != spriteMeshData.vertexCount)
                m_SmoothValues = new float[spriteMeshData.vertexCount];

            Array.Clear(m_SmoothValues, 0, m_SmoothValues.Length);

            m_SmoothedBoneWeights.Clear();

            BoneWeight[] boneWeights = new BoneWeight[spriteMeshData.vertexCount];

            for (int i = 0; i < spriteMeshData.vertexCount; i++)
            {
                EditableBoneWeight editableBoneWeight = spriteMeshData.GetWeight(i);
                boneWeights[i] = editableBoneWeight.ToBoneWeight(false);
            }

            m_SmoothedBoneWeights.Add(boneWeights);
        }

        private BoneWeight[] GetSmoothedBoneWeights(int lerpIndex)
        {
            Debug.Assert(lerpIndex >= 0);

            while (lerpIndex >= m_SmoothedBoneWeights.Count && lerpIndex <= maxSmoothIterations)
            {
                BoneWeight[] boneWeights;
                SmoothingUtility.SmoothWeights(m_SmoothedBoneWeights[m_SmoothedBoneWeights.Count - 1], spriteMeshData.indices, BoneCount, out boneWeights);
                m_SmoothedBoneWeights.Add(boneWeights);
            }

            return m_SmoothedBoneWeights[Mathf.Min(lerpIndex, maxSmoothIterations)];
        }

        private float GetLerpValue(float smoothValue)
        {
            Debug.Assert(smoothValue >= 0f);
            return smoothValue - Mathf.Floor(smoothValue);
        }

        private int GetLerpIndex(float smoothValue)
        {
            Debug.Assert(smoothValue >= 0f);
            return Mathf.RoundToInt(Mathf.Floor(smoothValue) + 1);
        }

        private void StoreBoneWeights()
        {
            Debug.Assert(selection != null);

            m_StoredBoneWeights.Clear();

            for (int i = 0; i < spriteMeshData.vertexCount; i++)
            {
                EditableBoneWeight editableBoneWeight = spriteMeshData.GetWeight(i);
                m_StoredBoneWeights.Add(editableBoneWeight.ToBoneWeight(false));
            }
        }

        private void RestoreBoneWeights()
        {
            Debug.Assert(selection != null);

            for (int i = 0; i < spriteMeshData.vertexCount; i++)
            {
                EditableBoneWeight editableBoneWeight = spriteMeshData.GetWeight(i);
                editableBoneWeight.SetFromBoneWeight(m_StoredBoneWeights[i]);
            }

            if (m_SmoothValues != null)
                Array.Clear(m_SmoothValues, 0, m_SmoothValues.Length);
        }
    }
}
