using UnityEditor;
using UnityEngine;
using UnityEditor.Sprites;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor.U2D
{
    public enum AngleRangeAction
    {
        SelectRange,
        ModifyRange,
        CreateRange,
        ModifySelector,
        RemoveRange,
    }

    public interface IAngleRangeView
    {
        int hoveredRangeIndex { get; }

        void RequestFocusIndex(int index);
        float GetAngleFromPosition(Rect rect, float angleOffset);
        void Repaint();
        void SetupLayout(Rect rect, float angleOffset, float radius);
        bool DoAngleRange(int index, Rect rect, float radius, float angleOffset, ref float start, ref float end, bool snap, bool disabled, Color gradientMin, Color gradientMid, Color gradientMax);
        bool DoSelectAngleRange(int currentSelected, out int newSelected);
        bool DoCreateRange(Rect rect, float radius, float angleOffset, float start, float end);
        void DoCreateRangeTooltip();
        bool DoSelector(Rect rect, float angleOffset, float radius, float angle, out float newAngle);
        bool DoRemoveRange();
        bool IsActionActive(AngleRangeAction action);
        bool IsActionTriggering(AngleRangeAction action);
        bool IsActionFinishing(AngleRangeAction action);
        void DrawAngleRangeOutline(Rect rect, float start, float end, float angleOffset, float radius);
    }

    public class AngleRangeView : IAngleRangeView
    {
        const string kDeleteCommandName = "Delete";
        const string kSoftDeleteCommandName = "SoftDelete";
        static readonly int kAngleRangeHashCode = "AngleRange".GetHashCode();
        static readonly int kCreateRangeHashCode = "CreateRange".GetHashCode();
        static readonly int kSelectorHashCode = "Selector".GetHashCode();

        private static Color kHightlightColor = new Color(0.25f, 0.5f, 1.0f);
        private static Color kNoKeboardFocusColor = new Color(0.3f, 0.5f, 0.65f);

        private static class Contents
        {
            public static readonly GUIContent addRangeTooltip = new GUIContent("", "Click to add a new range");
        }

        private int m_FocusedRangeControlID = -1;
        private int m_HoveredRangeIndex = -1;
        private int m_HoveredRangeID = -1;
        private int m_HoveredHandleID = -1;
        private int m_HotHandleID = -1;
        private int m_CreateRangeControlID = -1;
        private int m_SelectorControlID = -1;
        private int m_RequestFocusIndex = -1;

        public int hoveredRangeIndex { get { return m_HoveredRangeIndex; } }

        public void RequestFocusIndex(int index)
        {
            GUIUtility.keyboardControl = 0;
            m_RequestFocusIndex = index;
        }

        public float GetAngleFromPosition(Rect rect, float angleOffset)
        {
            return Mathf.RoundToInt(SpriteShapeHandleUtility.PosToAngle(Event.current.mousePosition, rect.center, -angleOffset));
        }

        public void Repaint()
        {
            HandleUtility.Repaint();
        }

        public void SetupLayout(Rect rect, float angleOffset, float radius)
        {
            m_CreateRangeControlID = GUIUtility.GetControlID(kCreateRangeHashCode, FocusType.Passive);
            m_SelectorControlID = GUIUtility.GetControlID(kSelectorHashCode, FocusType.Passive);

            LayoutCreateRange(rect, angleOffset, radius);

            if (Event.current.type == EventType.Layout)
            {
                m_HoveredRangeIndex = -1;
                m_HoveredRangeID = -1;
                m_HoveredHandleID = -1;

                if (GUIUtility.hotControl == 0)
                {
                    m_HotHandleID = -1;
                }
            }
        }

        private void LayoutCreateRange(Rect rect, float angleOffset, float radius)
        {
            if (Event.current.type == EventType.Layout)
            {
                var mousePosition = Event.current.mousePosition;
                var distance = SpriteShapeHandleUtility.DistanceToArcWidth(mousePosition, rect.center, 0f, 360f, radius, AngleRangeGUI.kRangeWidth, angleOffset);

                HandleUtility.AddControl(m_CreateRangeControlID, distance);
            }
        }

        public bool DoAngleRange(int index, Rect rect, float radius, float angleOffset, ref float start, ref float end, bool snap, bool disabled, Color gradientMin, Color gradientMid, Color gradientMax)
        {
            var changed = false;

            var controlID = GUIUtility.GetControlID(kAngleRangeHashCode, FocusType.Passive);
            var leftHandleId = GUIUtility.GetControlID(AngleRangeGUI.kLeftHandleHashCode, FocusType.Passive);
            var rightHandleId = GUIUtility.GetControlID(AngleRangeGUI.kRightHandleHashCode, FocusType.Passive);

            if (Event.current.type == EventType.Layout)
            {
                var distance = SpriteShapeHandleUtility.DistanceToArcWidth(Event.current.mousePosition, rect.center, start, end, radius, AngleRangeGUI.kRangeWidth, angleOffset);
                HandleUtility.AddControl(controlID, distance);

                if (HandleUtility.nearestControl == controlID)
                {
                    m_HoveredRangeIndex = index;
                    m_HoveredRangeID = controlID;
                }
            }

            if (IsActionTriggering(AngleRangeAction.ModifyRange))
            {
                m_HotHandleID = m_HoveredHandleID;
                GrabKeyboardFocus(controlID);
            }

            if (m_RequestFocusIndex == index)
            {
                GrabKeyboardFocus(controlID);

                if (Event.current.type == EventType.Repaint)
                    m_RequestFocusIndex = -1;
            }

            using (new EditorGUI.DisabledScope(disabled))
            {
                var midAngle = (end - start) * 0.5f + start;
                var t = 2f * (midAngle + 180f) / 360f;
                var color = gradientMin;

                if (t < 1f)
                    color = Color.Lerp(gradientMin, gradientMid, t);
                else
                    color = Color.Lerp(gradientMid, gradientMax, t - 1f);

                if (!disabled)
                {
                    color = kNoKeboardFocusColor;

                    if (HasKeyboardFocus())
                        color = kHightlightColor;
                }

                EditorGUI.BeginChangeCheck();

                AngleRangeGUI.AngleRangeField(rect, leftHandleId, rightHandleId, ref start, ref end, angleOffset, radius, snap, false, false, color);

                changed = EditorGUI.EndChangeCheck();
            }

            //Extra Layout from handles
            if (Event.current.type == EventType.Layout &&
                (HandleUtility.nearestControl == leftHandleId || HandleUtility.nearestControl == rightHandleId))
            {
                m_HoveredRangeIndex = index;
                m_HoveredRangeID = controlID;
                m_HoveredHandleID = HandleUtility.nearestControl;
            }

            return changed;
        }

        public bool DoSelectAngleRange(int currentSelected, out int newSelected)
        {
            newSelected = currentSelected;

            if (IsActionTriggering(AngleRangeAction.SelectRange))
            {
                newSelected = m_HoveredRangeIndex;
                GUI.changed = true;
                Repaint();

                HandleUtility.nearestControl = m_SelectorControlID;

                return true;
            }

            return false;
        }

        public bool DoCreateRange(Rect rect, float radius, float angleOffset, float start, float end)
        {
            if (IsActionTriggering(AngleRangeAction.CreateRange))
            {
                GUI.changed = true;
                HandleUtility.nearestControl = m_SelectorControlID;
                return true;
            }

            if (IsActionActive(AngleRangeAction.CreateRange))
            {
                DrawAngleRangeOutline(rect, start, end, angleOffset, radius);

                if (Event.current.type == EventType.MouseMove)
                    Repaint();
            }

            return false;
        }

        public void DoCreateRangeTooltip()
        {
            if (IsActionActive(AngleRangeAction.CreateRange))
            {
                var mousePosition = Event.current.mousePosition;
                EditorGUI.LabelField(new Rect(mousePosition, new Vector2(1f, 20f)), Contents.addRangeTooltip);
            }
        }

        public bool DoSelector(Rect rect, float angleOffset, float radius, float angle, out float newAngle)
        {
            EditorGUI.BeginChangeCheck();
            newAngle = AngleRangeGUI.AngleField(rect, m_SelectorControlID, angle, angleOffset, Vector2.down * 7.5f, angle, 15f, radius - AngleRangeGUI.kRangeWidth, true, true, false, SpriteShapeHandleUtility.PlayHeadCap);
            return EditorGUI.EndChangeCheck();
        }

        public bool DoRemoveRange()
        {
            EventType eventType = Event.current.type;

            if (IsActionTriggering(AngleRangeAction.RemoveRange))
            {
                Event.current.Use();
                GUI.changed = true;

                return true;
            }

            return false;
        }

        public bool IsActionActive(AngleRangeAction action)
        {
            if (GUIUtility.hotControl != 0)
                return false;

            if (action == AngleRangeAction.SelectRange)
                return HandleUtility.nearestControl == m_HoveredRangeID;

            if (action == AngleRangeAction.ModifyRange)
                return HandleUtility.nearestControl == m_HoveredHandleID;

            if (action == AngleRangeAction.CreateRange)
                return HandleUtility.nearestControl == m_CreateRangeControlID;

            if (action == AngleRangeAction.ModifySelector)
                return HandleUtility.nearestControl == m_SelectorControlID;

            if (action == AngleRangeAction.RemoveRange)
                return HasKeyboardFocus();

            return false;
        }

        public bool IsActionHot(AngleRangeAction action)
        {
            if (GUIUtility.hotControl == 0)
                return false;

            if (action == AngleRangeAction.ModifyRange)
                return GUIUtility.hotControl == m_HotHandleID;

            return false;
        }

        public bool IsActionTriggering(AngleRangeAction action)
        {
            if (!IsActionActive(action))
                return false;

            EventType eventType = Event.current.type;

            if (action == AngleRangeAction.RemoveRange)
            {
                if ((eventType == EventType.ValidateCommand || eventType == EventType.ExecuteCommand)
                    && (Event.current.commandName == kSoftDeleteCommandName || Event.current.commandName == kDeleteCommandName))
                {
                    if (eventType == EventType.ExecuteCommand)
                        return true;

                    Event.current.Use();
                }

                return false;
            }

            return eventType == EventType.MouseDown && Event.current.button == 0;
        }

        public bool IsActionFinishing(AngleRangeAction action)
        {
            if (!IsActionHot(action))
                return false;

            return (Event.current.type == EventType.MouseUp && Event.current.button == 0) || Event.current.type == EventType.Ignore;
        }

        public void DrawAngleRangeOutline(Rect rect, float start, float end, float angleOffset, float radius)
        {
            if (Event.current.type == EventType.Repaint)
                SpriteShapeHandleUtility.DrawRangeOutline(start, end, angleOffset, rect.center, radius, AngleRangeGUI.kRangeWidth - 1f);
        }

        private void GrabKeyboardFocus(int controlID)
        {
            m_FocusedRangeControlID = controlID;
            GUIUtility.keyboardControl = controlID;
        }

        private bool HasKeyboardFocus()
        {
            return GUIUtility.keyboardControl == m_FocusedRangeControlID;
        }
    }
}
