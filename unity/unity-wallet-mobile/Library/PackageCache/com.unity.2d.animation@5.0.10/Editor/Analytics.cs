#define WRITE_TO_JSON
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Analytics;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    [Serializable]
    enum AnimationToolType
    {
        UnknownTool = 0,
        Visibilility = 6,
        PreviewPose = 7,
        EditPose = 8,
        CreateBone = 9,
        SplitBone = 10,
        ReparentBone = 11,
        EditGeometry = 12,
        CreateVertex = 13,
        CreateEdge = 14,
        SplitEdge = 15,
        GenerateGeometry = 16,
        WeightSlider = 17,
        WeightBrush = 18,
        BoneInfluence = 19,
        GenerateWeights = 20
    }

    [Serializable]
    enum AnimationEventType
    {
        Truncated = -1,
        SelectedSpriteChanged = 0,
        SkeletonPreviewPoseChanged = 1,
        SkeletonBindPoseChanged = 2,
        SkeletonTopologyChanged = 3,
        MeshChanged = 4,
        MeshPreviewChanged = 5,
        SkinningModuleModeChanged = 6,
        BoneSelectionChanged = 7,
        BoneNameChanged = 8,
        CharacterPartChanged = 9,
        ToolChanged = 10,
        RestoreBindPose = 11,
        Copy = 12,
        Paste = 13,
        BoneDepthChanged = 14,
        Shortcut = 15,
        Visibility = 16
    }

    [Serializable]
    struct AnimationEvent
    {
        [SerializeField]
        public AnimationEventType sub_type;
        [SerializeField]
        public int repeated_event;
        [SerializeField]
        public string data;
    }

    [Serializable]
    struct AnimationToolUsageEvent
    {
        [SerializeField]
        public int instance_id;
        [SerializeField]
        public  AnimationToolType animation_tool;
        [SerializeField]
        public  bool character_mode;
        [SerializeField]
        public int time_start_s;
        [SerializeField]
        public int time_end_s;
        [SerializeField]
        public List<AnimationEvent> animation_events;
    }

    [Serializable]
    struct AnimationToolApplyEvent
    {
        [SerializeField]
        public  bool character_mode;
        [SerializeField]
        public int instance_id;
        [SerializeField]
        public int sprite_count;
        [SerializeField]
        public int[] bone_sprite_count;
        [SerializeField]
        public int[] bone_count;
        [SerializeField]
        public int[] bone_depth;
        [SerializeField]
        public int[] bone_chain_count;
        [SerializeField]
        public int bone_root_count;
    }

    internal interface IAnimationAnalyticsModel
    {
        bool hasCharacter { get; }
        SkinningMode mode { get; }
        ITool selectedTool { get; }
        ITool GetTool(Tools tool);
        int selectedBoneCount { get; }
        int applicationElapseTime { get; }
    }

    internal class SkinningModuleAnalyticsModel : IAnimationAnalyticsModel
    {
        public SkinningCache skinningCache { get; private set; }
        public bool hasCharacter { get { return skinningCache.hasCharacter; } }
        public SkinningMode mode { get { return skinningCache.mode; } }
        public ITool selectedTool { get { return skinningCache.selectedTool; } }

        public SkinningModuleAnalyticsModel(SkinningCache s)
        {
            skinningCache = s;
        }

        public ITool GetTool(Tools tool)
        {
            return skinningCache.GetTool(tool);
        }

        public int selectedBoneCount { get { return skinningCache.skeletonSelection.Count; } }

        public int applicationElapseTime { get { return (int)EditorApplication.timeSinceStartup; } }
    }

    [Serializable]
    internal class AnimationAnalytics
    {
        const int k_AnimationEventElementCount = 3;
        const int k_AnimationToolUsageEventElementCount = 6;
        IAnalyticsStorage m_AnalyticsStorage;
        SkinningEvents m_EventBus;
        IAnimationAnalyticsModel m_Model;

        AnimationToolUsageEvent? m_CurrentEvent;
        int m_InstanceId;

        public AnimationAnalytics(IAnalyticsStorage analyticsStorage, SkinningEvents eventBus, IAnimationAnalyticsModel model, int instanceId)
        {
            m_Model = model;
            m_AnalyticsStorage = analyticsStorage;
            m_InstanceId = instanceId;
            m_EventBus = eventBus;
            m_EventBus.selectedSpriteChanged.AddListener(OnSelectedSpriteChanged);
            m_EventBus.skeletonPreviewPoseChanged.AddListener(OnSkeletonPreviewPoseChanged);
            m_EventBus.skeletonBindPoseChanged.AddListener(OnSkeletonBindPoseChanged);
            m_EventBus.skeletonTopologyChanged.AddListener(OnSkeletonTopologyChanged);
            m_EventBus.meshChanged.AddListener(OnMeshChanged);
            m_EventBus.meshPreviewChanged.AddListener(OnMeshPreviewChanged);
            m_EventBus.skinningModeChanged.AddListener(OnSkinningModuleModeChanged);
            m_EventBus.boneSelectionChanged.AddListener(OnBoneSelectionChanged);
            m_EventBus.boneNameChanged.AddListener(OnBoneNameChanged);
            m_EventBus.boneDepthChanged.AddListener(OnBoneDepthChanged);
            m_EventBus.characterPartChanged.AddListener(OnCharacterPartChanged);
            m_EventBus.toolChanged.AddListener(OnToolChanged);
            m_EventBus.restoreBindPose.AddListener(OnRestoreBindPose);
            m_EventBus.copy.AddListener(OnCopy);
            m_EventBus.paste.AddListener(OnPaste);
            m_EventBus.shortcut.AddListener(OnShortcut);
            m_EventBus.boneVisibility.AddListener(OnBoneVisibility);

            OnToolChanged(model.selectedTool);
        }

        public void Dispose()
        {
            m_EventBus.selectedSpriteChanged.RemoveListener(OnSelectedSpriteChanged);
            m_EventBus.skeletonPreviewPoseChanged.RemoveListener(OnSkeletonPreviewPoseChanged);
            m_EventBus.skeletonBindPoseChanged.RemoveListener(OnSkeletonBindPoseChanged);
            m_EventBus.skeletonTopologyChanged.RemoveListener(OnSkeletonTopologyChanged);
            m_EventBus.meshChanged.RemoveListener(OnMeshChanged);
            m_EventBus.meshPreviewChanged.RemoveListener(OnMeshPreviewChanged);
            m_EventBus.skinningModeChanged.RemoveListener(OnSkinningModuleModeChanged);
            m_EventBus.boneSelectionChanged.RemoveListener(OnBoneSelectionChanged);
            m_EventBus.boneNameChanged.RemoveListener(OnBoneNameChanged);
            m_EventBus.boneDepthChanged.AddListener(OnBoneDepthChanged);
            m_EventBus.characterPartChanged.RemoveListener(OnCharacterPartChanged);
            m_EventBus.toolChanged.RemoveListener(OnToolChanged);
            m_EventBus.copy.RemoveListener(OnCopy);
            m_EventBus.paste.RemoveListener(OnPaste);
            m_EventBus.shortcut.RemoveListener(OnShortcut);
            m_EventBus.boneVisibility.RemoveListener(OnBoneVisibility);
            m_AnalyticsStorage.Dispose();
        }

        void OnBoneVisibility(string s)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.Visibility,
                data = s
            });
        }

        void OnShortcut(string s)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.Shortcut,
                data = s
            });
        }

        void OnCopy()
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.Copy,
                data = ""
            });
        }

        void OnPaste(bool bone , bool mesh , bool flipX , bool flipY)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.Paste,
                data = string.Format("b:{0} m:{1} x:{2} y:{3}", bone, mesh, flipX, flipY)
            });
        }

        void OnSelectedSpriteChanged(SpriteCache sprite)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.SelectedSpriteChanged,
                data = sprite == null ? "false" : "true"
            });
        }

        void OnSkeletonPreviewPoseChanged(SkeletonCache skeleton)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.SkeletonPreviewPoseChanged,
                data = ""
            });
        }

        void OnSkeletonBindPoseChanged(SkeletonCache skeleton)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.SkeletonBindPoseChanged,
                data = ""
            });
        }

        void OnSkeletonTopologyChanged(SkeletonCache skeleton)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.SkeletonTopologyChanged,
                data = ""
            });
        }

        void OnMeshChanged(MeshCache mesh)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.MeshChanged,
                data = ""
            });
        }

        void OnMeshPreviewChanged(MeshPreviewCache mesh)
        {
        }

        void OnSkinningModuleModeChanged(SkinningMode mode)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.SkinningModuleModeChanged,
                data = mode.ToString()
            });
        }

        void OnBoneSelectionChanged()
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.BoneSelectionChanged,
                data = m_Model.selectedBoneCount.ToString()
            });
        }

        void OnBoneNameChanged(BoneCache bone)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.BoneNameChanged,
                data = ""
            });
        }

        void OnBoneDepthChanged(BoneCache bone)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.BoneDepthChanged,
                data = ""
            });
        }

        void OnCharacterPartChanged(CharacterPartCache part)
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.CharacterPartChanged,
                data = ""
            });
        }

        void OnToolChanged(ITool tool)
        {
            if (tool == m_Model.GetTool(Tools.ReparentBone))
                StartNewEvent(AnimationToolType.ReparentBone, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.CreateBone))
                StartNewEvent(AnimationToolType.CreateBone, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.EditJoints))
                StartNewEvent(AnimationToolType.EditPose, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.EditPose))
                StartNewEvent(AnimationToolType.PreviewPose, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.SplitBone))
                StartNewEvent(AnimationToolType.SplitBone, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.CreateEdge))
                StartNewEvent(AnimationToolType.CreateEdge, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.CreateVertex))
                StartNewEvent(AnimationToolType.CreateVertex, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.EditGeometry))
                StartNewEvent(AnimationToolType.EditGeometry, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.GenerateGeometry))
                StartNewEvent(AnimationToolType.GenerateGeometry, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.SplitEdge))
                StartNewEvent(AnimationToolType.SplitEdge, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.Visibility))
                StartNewEvent(AnimationToolType.Visibilility, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.BoneInfluence))
                StartNewEvent(AnimationToolType.BoneInfluence, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.GenerateWeights))
                StartNewEvent(AnimationToolType.GenerateWeights, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.WeightBrush))
                StartNewEvent(AnimationToolType.WeightBrush, m_Model.applicationElapseTime);
            else if (tool == m_Model.GetTool(Tools.WeightSlider))
                StartNewEvent(AnimationToolType.WeightSlider, m_Model.applicationElapseTime);
            else
                StartNewEvent(AnimationToolType.UnknownTool, m_Model.applicationElapseTime);
        }

        void OnRestoreBindPose()
        {
            SetAnimationEvent(new AnimationEvent()
            {
                sub_type = AnimationEventType.RestoreBindPose,
                data = ""
            });
        }

        void SetAnimationEvent(AnimationEvent evt)
        {
            if (m_CurrentEvent != null)
            {
                var toolEvent = m_CurrentEvent.Value;
                var eventCount = toolEvent.animation_events.Count;
                if (eventCount > 0 && toolEvent.animation_events[eventCount - 1].sub_type == evt.sub_type && toolEvent.animation_events[eventCount - 1].data == evt.data)
                {
                    var e = toolEvent.animation_events[eventCount - 1];
                    e.repeated_event += 1;
                    toolEvent.animation_events[eventCount - 1] = e;
                }
                else
                {
                    var elementCountPlus = k_AnimationToolUsageEventElementCount + (eventCount + 1 * k_AnimationEventElementCount);
                    if (elementCountPlus >= AnalyticConstant.k_MaxNumberOfElements)
                    {
                        // We reached the max number of events. Change the last one to truncated
                        var e = toolEvent.animation_events[eventCount - 1];
                        if (e.sub_type != AnimationEventType.Truncated)
                        {
                            e.sub_type = AnimationEventType.Truncated;
                            e.repeated_event = 0;
                        }
                        e.repeated_event += 1;
                        toolEvent.animation_events[eventCount - 1] = e;
                    }
                    else
                        toolEvent.animation_events.Add(evt);
                }
                m_CurrentEvent = toolEvent;
            }
        }

        void StartNewEvent(AnimationToolType animationType, int tick)
        {
            SendLastEvent(tick);
            m_CurrentEvent = new AnimationToolUsageEvent()
            {
                instance_id = m_InstanceId,
                character_mode = m_Model.mode == SkinningMode.Character,
                animation_tool = animationType,
                time_start_s = tick,
                animation_events = new List<AnimationEvent>()
            };
        }

        void SendLastEvent(AnimationToolUsageEvent evt, int tick)
        {
            evt.time_end_s = tick;
            m_AnalyticsStorage.SendUsageEvent(evt);
        }

        void SendLastEvent(int tick)
        {
            if (m_CurrentEvent != null)
            {
                SendLastEvent(m_CurrentEvent.Value, tick);
            }
            m_CurrentEvent = null;
        }

        public void FlushEvent()
        {
            SendLastEvent(m_Model.applicationElapseTime);
        }

        public void SendApplyEvent(int spriteCount, int[] spriteBoneCount, BoneCache[] bones)
        {
            int[] chainBoneCount = null;
            int[] maxDepth = null;
            int[] boneCount = null;
            int boneRootCount = 0;
            GetChainBoneStatistic(bones, out chainBoneCount, out maxDepth, out boneRootCount, out boneCount);
            var applyEvent = new AnimationToolApplyEvent()
            {
                instance_id = m_InstanceId,
                character_mode = m_Model.hasCharacter,
                sprite_count = spriteCount,
                bone_sprite_count = spriteBoneCount,
                bone_depth = maxDepth,
                bone_chain_count = chainBoneCount,
                bone_root_count = boneRootCount,
                bone_count = boneCount
            };
            m_AnalyticsStorage.SendApplyEvent(applyEvent);
        }

        static void GetChainBoneStatistic(BoneCache[] bones, out int[] chainBoneCount, out int[] maxDepth, out int boneRootCount, out int[] boneCount)
        {
            List<int> chainCountList = new List<int>();
            List<int> boneDepthList = new List<int>();
            List<int> countList = new List<int>();
            boneRootCount = 0;
            foreach (var b in bones)
            {
                if (b.parentBone == null)
                {
                    ++boneRootCount;
                    var chain = 0;
                    var chainDepth = 0;
                    var tempBone = b;
                    var count = 1;
                    while (tempBone != null)
                    {
                        ++chainDepth;
                        tempBone = tempBone.chainedChild;
                    }

                    foreach (var b1 in bones)
                    {
                        // if this bone is part of this root
                        var parentBone = b1.parentBone;
                        while (parentBone != null)
                        {
                            if (parentBone == b)
                            {
                                ++count;
                                // the bone has a parent and the parent bone's chainedChild is not us, means we are a new chain
                                if (b1.parentBone != null && b1.parentBone.chainedChild != b1)
                                {
                                    ++chain;
                                    var chainDepth1 = 0;
                                    tempBone = b1;
                                    while (tempBone != null)
                                    {
                                        ++chainDepth1;
                                        tempBone = tempBone.chainedChild;
                                    }

                                    chainDepth = chainDepth1 > chainDepth ? chainDepth1 : chainDepth;
                                }
                                break;
                            }
                            parentBone = parentBone.parentBone;
                        }
                    }
                    chainCountList.Add(chain);
                    boneDepthList.Add(chainDepth);
                    countList.Add(count);
                }
            }
            chainBoneCount = chainCountList.ToArray();
            maxDepth = boneDepthList.ToArray();
            boneCount = countList.ToArray();
        }
    }

    internal interface IAnalyticsStorage
    {
        AnalyticsResult SendUsageEvent(AnimationToolUsageEvent evt);
        AnalyticsResult SendApplyEvent(AnimationToolApplyEvent evt);
        void Dispose();
    }

    internal static class AnalyticConstant
    {
        public const int k_MaxEventsPerHour = 1000;
        public  const int k_MaxNumberOfElements = 1000;
    }

    internal class AnalyticsJsonStorage : IAnalyticsStorage
    {
        [Serializable]
        struct AnimationToolEvents
        {
            [SerializeField]
            public List<AnimationToolUsageEvent> events;
            [SerializeField]
            public AnimationToolApplyEvent applyEvent;
        }

        AnimationToolEvents m_TotalEvents = new AnimationToolEvents()
        {
            events = new List<AnimationToolUsageEvent>(),
            applyEvent = new AnimationToolApplyEvent()
        };

        public AnalyticsResult SendUsageEvent(AnimationToolUsageEvent evt)
        {
            m_TotalEvents.events.Add(evt);
            return AnalyticsResult.Ok;
        }

        public AnalyticsResult SendApplyEvent(AnimationToolApplyEvent evt)
        {
            m_TotalEvents.applyEvent = evt;
            return AnalyticsResult.Ok;
        }

        public void Dispose()
        {
            try
            {
                string file = string.Format("analytics_{0}.json", System.DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
                System.IO.File.WriteAllText(file, JsonUtility.ToJson(m_TotalEvents, true));
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
            finally
            {
                m_TotalEvents.events.Clear();
            }
        }
    }

    [InitializeOnLoad]
    internal class UnityAnalyticsStorage : IAnalyticsStorage
    {
        const string k_VendorKey = "unity.2d.animation";
        const int k_Version = 1;

        static UnityAnalyticsStorage()
        {
            EditorAnalytics.RegisterEventWithLimit("u2dAnimationToolUsage", AnalyticConstant.k_MaxEventsPerHour, AnalyticConstant.k_MaxNumberOfElements, k_VendorKey, k_Version);
            EditorAnalytics.RegisterEventWithLimit("u2dAnimationToolApply", AnalyticConstant.k_MaxEventsPerHour, AnalyticConstant.k_MaxNumberOfElements, k_VendorKey, k_Version);
        }

        public AnalyticsResult SendUsageEvent(AnimationToolUsageEvent evt)
        {
            return EditorAnalytics.SendEventWithLimit("u2dAnimationToolUsage", evt, k_Version);
        }

        public AnalyticsResult SendApplyEvent(AnimationToolApplyEvent evt)
        {
            return EditorAnalytics.SendEventWithLimit("u2dAnimationToolApply", evt, k_Version);
        }

        public void Dispose() {}
    }
}
