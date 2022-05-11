using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal enum SkeletonAction
    {
        None = 0,
        Select = 1 << 0,
        RotateBone = 1 << 2,
        MoveBone = 1 << 3,
        FreeMoveBone = 1 << 4,
        MoveEndPosition = 1 << 5,
        MoveJoint = 1 << 6,
        ChangeLength = 1 << 7,
        CreateBone = 1 << 8,
        SplitBone = 1 << 9,
        Remove = 1 << 10,
    }

    internal enum SkeletonMode
    {
        Disabled = SkeletonAction.None,
        Selection = SkeletonAction.Select,
        EditPose = Selection | SkeletonAction.RotateBone | SkeletonAction.MoveBone,
        EditJoints = Selection | SkeletonAction.FreeMoveBone | SkeletonAction.MoveEndPosition | SkeletonAction.MoveJoint | SkeletonAction.Remove,
        CreateBone = Selection | SkeletonAction.MoveJoint | SkeletonAction.Remove | SkeletonAction.CreateBone,
        SplitBone = Selection | SkeletonAction.MoveEndPosition | SkeletonAction.MoveJoint | SkeletonAction.Remove | SkeletonAction.SplitBone,
    }

    internal interface ISkeletonView
    {
        int InvalidID { get; set; }
        SkeletonMode mode { get; set; }
        int defaultControlID { get; set; }
        int hoveredBoneID { get; }
        int hoveredJointID { get; }
        int hoveredBodyID { get; }
        int hoveredTailID { get; }
        int hotBoneID { get; }
        void BeginLayout();
        void EndLayout();
        bool CanLayout();
        Vector3 GetMouseWorldPosition(Vector3 planeNormal, Vector3 planePosition);
        void LayoutBone(int id, Vector3 position, Vector3 endPosition, Vector3 forward, Vector3 up, Vector3 right, bool isChainEnd);
        bool DoSelectBone(out int id, out bool additive);
        bool DoRotateBone(Vector3 pivot, Vector3 normal, out float deltaAngle);
        bool DoMoveBone(out Vector3 deltaPosition);
        bool DoFreeMoveBone(out Vector3 deltaPosition);
        bool DoMoveJoint(out Vector3 deltaPosition);
        bool DoMoveEndPosition(out Vector3 endPosition);
        bool DoChangeLength(out Vector3 endPosition);
        bool DoCreateBoneStart(out Vector3 position);
        bool DoCreateBone(out Vector3 position);
        bool DoSplitBone(out int id, out Vector3 position);
        bool DoRemoveBone();
        bool DoCancelMultistepAction(bool force);
        bool IsActionActive(SkeletonAction action);
        bool IsActionHot(SkeletonAction action);
        bool IsActionTriggering(SkeletonAction action);
        bool IsActionFinishing(SkeletonAction action);
        bool IsRepainting();
        void DrawBone(Vector3 position, Vector3 right, Vector3 forward, float length, Color color, bool isChained, bool isSelected, bool isJointHovered, bool isTailHovered, bool isHot);
        void DrawBoneParentLink(Vector3 parentPosition, Vector3 position, Vector3 forward, Color color);
        void DrawBoneOutline(Vector3 position, Vector3 right, Vector3 forward, float length, Color color, float outlineScale);
        void DrawCursors(bool canBeActive);
    }
}
