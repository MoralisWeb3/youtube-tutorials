using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace UnityEditor.U2D.SpriteShape
{
    public interface IAngleRangeCache
    {
        List<AngleRange> angleRanges { get; }
        int selectedIndex { get; set; }
        float previewAngle { get; set; }
        void RegisterUndo(string name);
    }

    public class AngleRangeController
    {
        public event Action selectionChanged = () => { };
        public IAngleRangeCache cache { get; set; }
        public IAngleRangeView view { get; set; }
        public float angleOffset { get; set; }
        public float radius { get; set; }
        public Rect rect { get; set; }
        public bool snap { get; set; }
        public Color gradientMin { get; set; }
        public Color gradientMid { get; set; }
        public Color gradientMax { get; set; }

        public AngleRange selectedAngleRange
        {
            get
            {
                Debug.Assert(cache != null);

                AngleRange angleRange = null;

                if (cache.selectedIndex >= 0 && cache.selectedIndex < cache.angleRanges.Count)
                    return cache.angleRanges[cache.selectedIndex];

                return angleRange;
            }
        }

        private AngleRange hoveredAngleRange
        {
            get
            {
                Debug.Assert(cache != null);
                Debug.Assert(view != null);

                AngleRange angleRange = null;

                if (view.hoveredRangeIndex >= 0 && view.hoveredRangeIndex < cache.angleRanges.Count)
                    return cache.angleRanges[view.hoveredRangeIndex];

                return angleRange;
            }
        }

        public void OnGUI()
        {
            Debug.Assert(cache != null);

            view.SetupLayout(rect, angleOffset, radius);

            DoAngleRanges();
            HandleSelectAngleRange();
            HandleCreateRange();
            HandlePreviewSelector();
            HandleRemoveRange();
        }

        private void DoAngleRanges()
        {
            var removeInvalid = false;

            if (view.IsActionTriggering(AngleRangeAction.ModifyRange))
                cache.RegisterUndo("Modify Range");

            if (view.IsActionFinishing(AngleRangeAction.ModifyRange))
                removeInvalid = true;

            var index = 0;
            foreach (var angleRange in cache.angleRanges)
            {
                var start = angleRange.start;
                var end = angleRange.end;
                var isSelected = selectedAngleRange != null && selectedAngleRange == angleRange;

                if (view.DoAngleRange(index, rect, radius, angleOffset, ref start, ref end, snap, !isSelected, gradientMin, gradientMid, gradientMax))
                    SetRange(angleRange, start, end);

                ++index;
            }

            if (removeInvalid)
                RemoveInvalidRanges();
        }

        public void RemoveInvalidRanges()
        {
            var toDelete = new List<AngleRange>();

            foreach (var angleRange in cache.angleRanges)
            {
                var start = angleRange.start;
                var end = angleRange.end;

                if (start >= end)
                    toDelete.Add(angleRange);
            }

            foreach (var angleRange in toDelete)
                cache.angleRanges.Remove(angleRange);

            if (toDelete.Count > 0)
            {
                SetSelectedIndexFromPreviewAngle();
                view.Repaint();
            }
        }

        private void HandleSelectAngleRange()
        {
            int newSelected;
            if (view.DoSelectAngleRange(cache.selectedIndex, out newSelected))
            {
                cache.RegisterUndo("Select Angle Range");
                cache.previewAngle = view.GetAngleFromPosition(rect, angleOffset);
                SelectIndex(newSelected);
            }

            if (view.IsActionActive(AngleRangeAction.SelectRange))
            {
                if (hoveredAngleRange == null)
                    return;

                view.DrawAngleRangeOutline(rect, hoveredAngleRange.start, hoveredAngleRange.end, angleOffset, radius);
            }
        }

        private void HandleCreateRange()
        {
            if (!view.IsActionActive(AngleRangeAction.CreateRange))
                return;

            var angle = view.GetAngleFromPosition(rect, angleOffset);
            var start = 0f;
            var end = 0f;
            var canCreate = GetNewRangeBounds(angle, out start, out end);

            if (canCreate && view.DoCreateRange(rect, radius, angleOffset, start, end))
            {
                CreateRangeAtAngle(angle);
                cache.previewAngle = angle;
                SetSelectedIndexFromPreviewAngle();
            }
        }

        private void HandlePreviewSelector()
        {
            if (view.IsActionTriggering(AngleRangeAction.ModifySelector))
                cache.RegisterUndo("Set Preview Angle");

            float newAngle;
            if (view.DoSelector(rect, angleOffset, radius, cache.previewAngle, out newAngle))
            {
                if (selectedAngleRange == null)
                    newAngle = Mathf.Repeat(newAngle + 180f, 360f) - 180f;

                cache.previewAngle = newAngle;
                SetSelectedIndexFromPreviewAngle();
            }
        }

        private void SetSelectedIndexFromPreviewAngle()
        {
            var index = SpriteShapeEditorUtility.GetRangeIndexFromAngle(cache.angleRanges, cache.previewAngle);
            SelectIndex(index);
        }

        private void SelectIndex(int index)
        {
            view.RequestFocusIndex(index);

            if (cache.selectedIndex == index)
                return;

            cache.selectedIndex = index;
            selectionChanged();
        }

        private void ClampPreviewAngle(float start, float end, float prevStart, float prevEnd)
        {
            var angle = cache.previewAngle;

            if (prevStart < start)
            {
                var a1 = Mathf.Repeat(angle - prevStart, 360f);
                var a2 = Mathf.Repeat(angle - start, 360f);

                if (a1 < a2)
                    angle = Mathf.Min(start, end);
            }
            else if (prevEnd > end)
            {
                var a1 = Mathf.Repeat(prevEnd - angle, 360f);
                var a2 = Mathf.Repeat(end - angle, 360f);

                if (a1 < a2)
                    angle = Mathf.Max(start, end);
            }

            cache.previewAngle = angle;
        }

        private void HandleRemoveRange()
        {
            if (view.DoRemoveRange())
            {
                cache.RegisterUndo("Remove Range");
                cache.angleRanges.RemoveAt(cache.selectedIndex);
                SelectIndex(-1);
            }
        }

        public void CreateRange()
        {
            CreateRangeAtAngle(cache.previewAngle);
        }

        private void CreateRangeAtAngle(float angle)
        {
            var start = 0f;
            var end = 0f;

            if (GetNewRangeBounds(angle, out start, out end))
            {
                cache.RegisterUndo("Create Range");

                var angleRange = new AngleRange();
                angleRange.start = start;
                angleRange.end = end;

                cache.angleRanges.Add(angleRange);

                ValidateRange(angleRange);
                SetSelectedIndexFromPreviewAngle();
            }
        }

        public void SetRange(AngleRange angleRange, float start, float end)
        {
            var prevStart = angleRange.start;
            var prevEnd = angleRange.end;

            angleRange.start = start;
            angleRange.end = end;

            ValidateRange(angleRange, prevStart, prevEnd);

            if (angleRange == selectedAngleRange)
                ClampPreviewAngle(start, end, prevStart, prevEnd);
        }

        private bool GetNewRangeBounds(float angle, out float emptyRangeStart, out float emptyRangeEnd)
        {
            angle = Mathf.Repeat(angle + 180f, 360f) - 180f;

            emptyRangeStart = float.MinValue;
            emptyRangeEnd = float.MaxValue;

            if (GetAngleRangeAt(angle) != null)
                return false;

            FindMinMax(out emptyRangeEnd, out emptyRangeStart);

            if (angle < emptyRangeStart)
                emptyRangeStart -= 360f;

            if (angle > emptyRangeEnd)
                emptyRangeEnd += 360f;

            foreach (var angleRange in cache.angleRanges)
            {
                var start = angleRange.start;
                var end = angleRange.end;

                if (angle > end)
                    emptyRangeStart = Mathf.Max(emptyRangeStart, end);

                if (angle < start)
                    emptyRangeEnd = Mathf.Min(emptyRangeEnd, start);
            }

            var rangeLength = emptyRangeEnd - emptyRangeStart;

            if (rangeLength > 90f)
            {
                emptyRangeStart = Mathf.Max(angle - 45f, emptyRangeStart);
                emptyRangeEnd = Mathf.Min(angle + 45f, emptyRangeEnd);
            }

            return true;
        }

        private AngleRange GetAngleRangeAt(float angle)
        {
            foreach (var angleRange in cache.angleRanges)
            {
                var start = angleRange.start;
                var end = angleRange.end;
                var range = end - start;

                var angle2 = Mathf.Repeat(angle - start, 360f);

                if (angle2 >= 0f && angle2 <= range)
                    return angleRange;
            }

            return null;
        }

        private void FindMinMax(out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            foreach (var angleRange in cache.angleRanges)
            {
                min = Mathf.Min(angleRange.start, min);
                max = Mathf.Max(angleRange.end, max);
            }
        }

        private void ValidateRange(AngleRange range)
        {
            ValidateRange(range, range.start, range.end);
        }

        private void ValidateRange(AngleRange angleRange, float prevStart, float prevEnd)
        {
            var start = angleRange.start;
            var end = angleRange.end;

            foreach (var otherRange in cache.angleRanges)
            {
                var otherStart = otherRange.start;
                var otherEnd = otherRange.end;

                if (otherRange == angleRange)
                {
                    if ((start > 180f && end > 180f) || (start < -180f && end < -180f))
                    {
                        start = Mathf.Repeat(start + 180f, 360f) - 180f;
                        end = Mathf.Repeat(end + 180f, 360f) - 180f;
                    }

                    otherStart = start + 360f;
                    otherEnd = end - 360f;
                }

                ValidateRangeStartEnd(ref start, ref end, prevStart, prevEnd, otherStart, otherEnd);
            }

            angleRange.start = start;
            angleRange.end = end;
        }

        private void ValidateRangeStartEnd(ref float start, ref float end, float prevStart, float prevEnd, float otherStart, float otherEnd)
        {
            var min = Mathf.Min(start, otherStart);
            var max = Mathf.Max(end, otherEnd);

            start -= min;
            end -= min;
            otherStart -= min;
            otherEnd -= min;
            prevStart -= min;
            prevEnd -= min;

            if (prevEnd != end)
                end = Mathf.Clamp(end, start, otherStart >= start ? otherStart : 360f);

            start += min - max;
            end += min - max;
            otherStart += min - max;
            otherEnd += min - max;
            prevStart += min - max;
            prevEnd += min - max;

            if (prevStart != start)
                start = Mathf.Clamp(start, otherEnd <= end ? otherEnd : -360f, end);

            start += max;
            end += max;
        }
    }
}
