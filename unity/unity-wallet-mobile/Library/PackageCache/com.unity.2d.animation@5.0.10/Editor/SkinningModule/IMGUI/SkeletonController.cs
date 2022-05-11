using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    [Serializable]
    internal class SkeletonController
    {
        private static readonly string k_DefaultRootName = "root";
        private static readonly string k_DefaultBoneName = "bone";
        private static Regex s_Regex = new Regex(@"\w+_\d+$", RegexOptions.IgnoreCase);

        private SkeletonCache m_Skeleton;
        [SerializeField]
        private Vector3 m_CreateBoneStartPosition;
        [SerializeField]
        private BoneCache m_PrevCreatedBone;
        private bool m_Moved = false;
        private ISkeletonStyle style
        {
            get
            {
                if (styleOverride != null)
                    return styleOverride;

                return SkeletonStyles.Default;
            }
        }
        private SkinningCache skinningCache
        {
            get { return m_Skeleton.skinningCache; }
        }
        private BoneCache selectedBone
        {
            get { return selection.activeElement.ToSpriteSheetIfNeeded(); }
            set { selection.activeElement = value.ToCharacterIfNeeded(); }
        }
        private BoneCache[] selectedBones
        {
            get { return selection.elements.ToSpriteSheetIfNeeded(); }
            set { selection.elements = value.ToCharacterIfNeeded(); }
        }
        private BoneCache rootBone
        {
            get { return selection.root.ToSpriteSheetIfNeeded(); }
        }
        private BoneCache[] rootBones
        {
            get { return selection.roots.ToSpriteSheetIfNeeded(); }
        }

        public ISkeletonView view { get; set; }
        public ISkeletonStyle styleOverride { get; set; }
        public IBoneSelection selection { get; set; }
        public bool editBindPose { get; set; }
        public SkeletonCache skeleton
        {
            get { return m_Skeleton; }
            set { SetSkeleton(value); }
        }
        public BoneCache hoveredBone
        {
            get { return GetBone(view.hoveredBoneID); }
        }
        public BoneCache hoveredTail
        {
            get { return GetBone(view.hoveredTailID); }
        }
        public BoneCache hoveredBody
        {
            get { return GetBone(view.hoveredBodyID); }
        }
        public BoneCache hoveredJoint
        {
            get { return GetBone(view.hoveredJointID); }
        }
        public BoneCache hotBone
        {
            get { return GetBone(view.hotBoneID); }
        }

        private BoneCache GetBone(int instanceID)
        {
            return BaseObject.InstanceIDToObject(instanceID) as BoneCache;
        }

        private void SetSkeleton(SkeletonCache newSkeleton)
        {
            if (skeleton != newSkeleton)
            {
                m_Skeleton = newSkeleton;
                Reset();
            }
        }

        public void Reset()
        {
            view.DoCancelMultistepAction(true);
        }

        public void OnGUI()
        {
            if (skeleton == null)
                return;

            view.BeginLayout();

            if (view.CanLayout())
                LayoutBones();

            view.EndLayout();

            HandleSelectBone();
            HandleRotateBone();
            HandleMoveBone();
            HandleFreeMoveBone();
            HandleMoveJoint();
            HandleMoveEndPosition();
            HandleChangeLength();
            HandleCreateBone();
            HandleSplitBone();
            HandleRemoveBone();
            HandleCancelMultiStepAction();
            DrawSkeleton();
            DrawSplitBonePreview();
            DrawCreateBonePreview();
            DrawCursors();
        }

        private void LayoutBones()
        {
            for (var i = 0; i < skeleton.BoneCount; ++i)
            {
                var bone = skeleton.GetBone(i);

                if (bone.isVisible && bone != hotBone)
                    view.LayoutBone(bone.GetInstanceID(), bone.position, bone.endPosition, bone.forward, bone.up, bone.right, bone.chainedChild == null);
            }
        }

        private void HandleSelectBone()
        {
            int instanceID;
            bool additive;
            if (view.DoSelectBone(out instanceID, out additive))
            {
                var bone = GetBone(instanceID).ToCharacterIfNeeded();

                using (skinningCache.UndoScope(TextContent.boneSelection, true))
                {
                    if (!additive)
                    {
                        if (!selection.Contains(bone))
                            selectedBone = bone;
                    }
                    else
                        selection.Select(bone, !selection.Contains(bone));

                    skinningCache.events.boneSelectionChanged.Invoke();
                }
            }
        }

        private void HandleRotateBone()
        {
            if (view.IsActionTriggering(SkeletonAction.RotateBone))
                m_Moved = false;

            var pivot = hoveredBone;

            if (view.IsActionHot(SkeletonAction.RotateBone))
                pivot = hotBone;

            if (pivot == null)
                return;

            var rootBones = selection.roots.ToSpriteSheetIfNeeded();
            pivot = pivot.FindRoot<BoneCache>(rootBones);

            if (pivot == null)
                return;

            float deltaAngle;
            if (view.DoRotateBone(pivot.position, pivot.forward, out deltaAngle))
            {
                if (!m_Moved)
                {
                    skinningCache.BeginUndoOperation(TextContent.rotateBone);
                    m_Moved = true;
                }

                m_Skeleton.RotateBones(selectedBones, deltaAngle);
                InvokePoseChanged();
            }
        }

        private void HandleMoveBone()
        {
            if (view.IsActionTriggering(SkeletonAction.MoveBone))
                m_Moved = false;

            Vector3 deltaPosition;
            if (view.DoMoveBone(out deltaPosition))
            {
                if (!m_Moved)
                {
                    skinningCache.BeginUndoOperation(TextContent.moveBone);
                    m_Moved = true;
                }

                m_Skeleton.MoveBones(rootBones, deltaPosition);
                InvokePoseChanged();
            }
        }

        private void HandleFreeMoveBone()
        {
            if (view.IsActionTriggering(SkeletonAction.FreeMoveBone))
                m_Moved = false;

            Vector3 deltaPosition;
            if (view.DoFreeMoveBone(out deltaPosition))
            {
                if (!m_Moved)
                {
                    skinningCache.BeginUndoOperation(TextContent.freeMoveBone);
                    m_Moved = true;
                }

                m_Skeleton.FreeMoveBones(selectedBones, deltaPosition);
                InvokePoseChanged();
            }
        }

        private void HandleMoveJoint()
        {
            if (view.IsActionTriggering(SkeletonAction.MoveJoint))
                m_Moved = false;

            if (view.IsActionFinishing(SkeletonAction.MoveJoint))
            {
                if (hoveredTail != null && hoveredTail.chainedChild == null && hotBone.parent == hoveredTail)
                    hoveredTail.chainedChild = hotBone;
            }

            Vector3 deltaPosition;
            if (view.DoMoveJoint(out deltaPosition))
            {
                if (!m_Moved)
                {
                    skinningCache.BeginUndoOperation(TextContent.moveJoint);
                    m_Moved = true;
                }

                //Snap to parent endPosition
                if (hoveredTail != null && hoveredTail.chainedChild == null && hotBone.parent == hoveredTail)
                    deltaPosition = hoveredTail.endPosition - hotBone.position;

                m_Skeleton.MoveJoints(selectedBones, deltaPosition);
                InvokePoseChanged();
            }
        }

        private void HandleMoveEndPosition()
        {
            if (view.IsActionTriggering(SkeletonAction.MoveEndPosition))
                m_Moved = false;

            if (view.IsActionFinishing(SkeletonAction.MoveEndPosition))
            {
                if (hoveredJoint != null && hoveredJoint.parent == hotBone)
                    hotBone.chainedChild = hoveredJoint;
            }

            Vector3 endPosition;
            if (view.DoMoveEndPosition(out endPosition))
            {
                if (!m_Moved)
                {
                    skinningCache.BeginUndoOperation(TextContent.moveEndPoint);
                    m_Moved = true;
                }

                Debug.Assert(hotBone != null);
                Debug.Assert(hotBone.chainedChild == null);

                if (hoveredJoint != null && hoveredJoint.parent == hotBone)
                    endPosition = hoveredJoint.position;
                
                m_Skeleton.SetEndPosition(hotBone, endPosition);
                InvokePoseChanged();
            }
        }

        private void HandleChangeLength()
        {
            if (view.IsActionTriggering(SkeletonAction.ChangeLength))
                m_Moved = false;

            Vector3 endPosition;
            if (view.DoChangeLength(out endPosition))
            {
                if (!m_Moved)
                {
                    skinningCache.BeginUndoOperation(TextContent.boneLength);
                    m_Moved = true;
                }

                Debug.Assert(hotBone != null);

                var direction = (Vector3)endPosition - hotBone.position;
                hotBone.length = Vector3.Dot(direction, hotBone.right);

                InvokePoseChanged();
            }
        }

        private void HandleCreateBone()
        {
            Vector3 position;
            if (view.DoCreateBoneStart(out position))
            {
                m_PrevCreatedBone = null;

                if (hoveredTail != null)
                {
                    m_PrevCreatedBone = hoveredTail;
                    m_CreateBoneStartPosition = hoveredTail.endPosition;
                }
                else
                {
                    m_CreateBoneStartPosition = position;
                }
            }

            if (view.DoCreateBone(out position))
            {
                using (skinningCache.UndoScope(TextContent.createBone))
                {
                    var isChained = m_PrevCreatedBone != null;
                    var parentBone = isChained ? m_PrevCreatedBone : rootBone;

                    if (isChained)
                        m_CreateBoneStartPosition = m_PrevCreatedBone.endPosition;

                    var name = AutoBoneName(parentBone, skeleton.bones);
                    var bone = m_Skeleton.CreateBone(parentBone, m_CreateBoneStartPosition, position, isChained, name);

                    m_PrevCreatedBone = bone;
                    m_CreateBoneStartPosition = bone.endPosition;

                    InvokeTopologyChanged();
                    InvokePoseChanged();
                }
            }
        }

        private void HandleSplitBone()
        {
            int instanceID;
            Vector3 position;
            if (view.DoSplitBone(out instanceID, out position))
            {
                using (skinningCache.UndoScope(TextContent.splitBone))
                {
                    var boneToSplit = GetBone(instanceID);

                    Debug.Assert(boneToSplit != null);

                    var splitLength = Vector3.Dot(hoveredBone.right, position - boneToSplit.position);
                    var name = AutoBoneName(boneToSplit, skeleton.bones);

                    m_Skeleton.SplitBone(boneToSplit, splitLength, name);

                    InvokeTopologyChanged();
                    InvokePoseChanged();
                }
            }
        }

        private void HandleRemoveBone()
        {
            if (view.DoRemoveBone())
            {
                using (skinningCache.UndoScope(TextContent.removeBone))
                {
                    m_Skeleton.DestroyBones(selectedBones);

                    selection.Clear();
                    skinningCache.events.boneSelectionChanged.Invoke();
                    InvokeTopologyChanged();
                    InvokePoseChanged();
                }
            }
        }

        private void HandleCancelMultiStepAction()
        {
            if (view.DoCancelMultistepAction(false))
                m_PrevCreatedBone = null;
        }

        private void DrawSkeleton()
        {
            if (!view.IsRepainting())
                return;

            bool isNotOnVisualElement = !skinningCache.IsOnVisualElement();

            if (view.IsActionActive(SkeletonAction.CreateBone) || view.IsActionHot(SkeletonAction.CreateBone))
            {
                if (isNotOnVisualElement)
                {
                    var endPoint = view.GetMouseWorldPosition(Vector3.forward, Vector3.zero);

                    if (view.IsActionHot(SkeletonAction.CreateBone))
                        endPoint = m_CreateBoneStartPosition;

                    if (m_PrevCreatedBone == null && hoveredTail == null)
                    {
                        var root = rootBone;
                        if (root != null)
                            view.DrawBoneParentLink(root.position, endPoint, Vector3.forward, style.GetParentLinkPreviewColor(skeleton.BoneCount));
                    }
                }
            }

            for (var i = 0; i < skeleton.BoneCount; ++i)
            {
                var bone = skeleton.GetBone(i);

                if (bone.isVisible == false || bone.parentBone == null || bone.parentBone.chainedChild == bone)
                    continue;

                view.DrawBoneParentLink(bone.parent.position, bone.position, Vector3.forward, style.GetParentLinkColor(bone));
            }

            for (var i = 0; i < skeleton.BoneCount; ++i)
            {
                var bone = skeleton.GetBone(i);

                if ((view.IsActionActive(SkeletonAction.SplitBone) && hoveredBone == bone && isNotOnVisualElement) || bone.isVisible == false)
                    continue;

                var isSelected = selection.Contains(bone.ToCharacterIfNeeded());
                var isHovered = hoveredBody == bone && view.IsActionHot(SkeletonAction.None) && isNotOnVisualElement;

                DrawBoneOutline(bone, style.GetOutlineColor(bone, isSelected, isHovered), style.GetOutlineScale(isSelected));
            }

            for (var i = 0; i < skeleton.BoneCount; ++i)
            {
                var bone = skeleton.GetBone(i);

                if ((view.IsActionActive(SkeletonAction.SplitBone) && hoveredBone == bone && isNotOnVisualElement) || bone.isVisible == false)
                    continue;

                DrawBone(bone, style.GetColor(bone));
            }
        }

        private void DrawBone(BoneCache bone, Color color)
        {
            var isSelected = selection.Contains(bone.ToCharacterIfNeeded());
            var isNotOnVisualElement = !skinningCache.IsOnVisualElement();
            var isJointHovered = view.IsActionHot(SkeletonAction.None) && hoveredJoint == bone && isNotOnVisualElement;
            var isTailHovered = view.IsActionHot(SkeletonAction.None) && hoveredTail == bone && isNotOnVisualElement;

            view.DrawBone(bone.position, bone.right, Vector3.forward, bone.length, color, bone.chainedChild != null, isSelected, isJointHovered, isTailHovered, bone == hotBone);
        }

        private void DrawBoneOutline(BoneCache bone, Color color, float outlineScale)
        {
            view.DrawBoneOutline(bone.position, bone.right, Vector3.forward, bone.length, color, outlineScale);
        }

        private void DrawSplitBonePreview()
        {
            if (!view.IsRepainting())
                return;

            if (skinningCache.IsOnVisualElement())
                return;

            if (view.IsActionActive(SkeletonAction.SplitBone) && hoveredBone != null)
            {
                var splitLength = Vector3.Dot(hoveredBone.right, view.GetMouseWorldPosition(hoveredBone.forward, hoveredBody.position) - hoveredBone.position);
                var position = hoveredBone.position + hoveredBone.right * splitLength;
                var length = hoveredBone.length - splitLength;
                var isSelected = selection.Contains(hoveredBone.ToCharacterIfNeeded());

                {
                    var color = style.GetOutlineColor(hoveredBone, false, false);
                    if (color.a > 0f)
                        view.DrawBoneOutline(hoveredBone.position, hoveredBone.right, Vector3.forward, splitLength, style.GetOutlineColor(hoveredBone, isSelected, true), style.GetOutlineScale(false));
                    
                }
                {
                    var color = style.GetPreviewOutlineColor(skeleton.BoneCount);
                    if (color.a > 0f)
                        view.DrawBoneOutline(position, hoveredBone.right, Vector3.forward, length, style.GetPreviewOutlineColor(skeleton.BoneCount), style.GetOutlineScale(false));
                    
                }

                view.DrawBone(hoveredBone.position,
                    hoveredBone.right,
                    Vector3.forward,
                    splitLength,
                    style.GetColor(hoveredBone),
                    hoveredBone.chainedChild != null,
                    false, false, false, false);
                view.DrawBone(position,
                    hoveredBone.right,
                    Vector3.forward,
                    length,
                    style.GetPreviewColor(skeleton.BoneCount),
                    hoveredBone.chainedChild != null,
                    false, false, false, false);
            }
        }

        private void DrawCreateBonePreview()
        {
            if (!view.IsRepainting())
                return;

            if (skinningCache.IsOnVisualElement())
                return;

            var color = style.GetPreviewColor(skeleton.BoneCount);
            var outlineColor = style.GetPreviewOutlineColor(skeleton.BoneCount);

            var startPosition = m_CreateBoneStartPosition;
            var mousePosition = view.GetMouseWorldPosition(Vector3.forward, Vector3.zero);

            if (view.IsActionActive(SkeletonAction.CreateBone))
            {
                startPosition = mousePosition;

                if (hoveredTail != null)
                    startPosition = hoveredTail.endPosition;

                if (outlineColor.a > 0f)
                    view.DrawBoneOutline(startPosition, Vector3.right, Vector3.forward, 0f, outlineColor, style.GetOutlineScale(false));

                view.DrawBone(startPosition, Vector3.right, Vector3.forward, 0f, color, false, false, false, false, false);
            }

            if (view.IsActionHot(SkeletonAction.CreateBone))
            {
                var direction = (mousePosition - startPosition);

                if (outlineColor.a > 0f)
                    view.DrawBoneOutline(startPosition, direction.normalized, Vector3.forward, direction.magnitude, outlineColor, style.GetOutlineScale(false));

                view.DrawBone(startPosition, direction.normalized, Vector3.forward, direction.magnitude, color, false, false, false, false, false);
            }
        }

        private void DrawCursors()
        {
            if (!view.IsRepainting())
                return;

            view.DrawCursors(!skinningCache.IsOnVisualElement());
        }

        public static string AutoBoneName(BoneCache parent, IEnumerable<BoneCache> bones)
        {
            string parentName = "root";
            string inheritedName;
            int counter;

            if (parent != null)
                parentName = parent.name;

            DissectBoneName(parentName, out inheritedName, out counter);
            int nameCounter = FindBiggestNameCounter(bones);

            if (inheritedName == k_DefaultRootName)
                inheritedName = k_DefaultBoneName;

            return String.Format("{0}_{1}", inheritedName, ++nameCounter);
        }

        private static int FindBiggestNameCounter(IEnumerable<BoneCache> bones)
        {
            int autoNameCounter = 0;
            string inheritedName;
            int counter;
            foreach (var bone in bones)
            {
                DissectBoneName(bone.name, out inheritedName, out counter);
                if (counter > autoNameCounter)
                    autoNameCounter = counter;
            }
            return autoNameCounter;
        }

        private static void DissectBoneName(string boneName, out string inheritedName, out int counter)
        {
            if (IsBoneNameMatchAutoFormat(boneName))
            {
                var tokens = boneName.Split('_');
                var lastTokenIndex = tokens.Length - 1;

                var tokensWithoutLast = new string[lastTokenIndex];
                Array.Copy(tokens, tokensWithoutLast, lastTokenIndex);
                inheritedName = string.Join("_", tokensWithoutLast);
                counter = int.Parse(tokens[lastTokenIndex]);
            }
            else
            {
                inheritedName = boneName;
                counter = -1;
            }
        }

        private static bool IsBoneNameMatchAutoFormat(string boneName)
        {
            return s_Regex.IsMatch(boneName);
        }

        private void InvokeTopologyChanged()
        {
            skinningCache.events.skeletonTopologyChanged.Invoke(skeleton);
        }

        private void InvokePoseChanged()
        {
            skeleton.SetPosePreview();

            if (editBindPose)
            {
                skeleton.SetDefaultPose();
                skinningCache.events.skeletonBindPoseChanged.Invoke(skeleton);
            }
            else
                skinningCache.events.skeletonPreviewPoseChanged.Invoke(skeleton);
        }
    }
}
