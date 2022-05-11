using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Path.GUIFramework;

namespace UnityEditor.U2D.Path
{
    public class PathEditor
    {
        const float kSnappingDistance = 15f;
        const string kDeleteCommandName = "Delete";
        const string kSoftDeleteCommandName = "SoftDelete";
        public IEditablePathController controller { get; set; }
        public bool linearTangentIsZero { get; set; }
        private IDrawer m_Drawer = new Drawer();
        private IDrawer m_DrawerOverride;
        private GUISystem m_GUISystem;

        public IDrawer drawerOverride { get; set; }

        private IDrawer drawer
        {
            get
            {
                if (drawerOverride != null)
                    return drawerOverride;

                return m_Drawer;
            }
        }

        public PathEditor() : this(new GUISystem(new GUIState())) { }
        
        public PathEditor(GUISystem guiSystem)
        {
            m_GUISystem = guiSystem;

            var m_PointControl = new GenericControl("Point")
            {
                count = GetPointCount,
                distance = (guiState, i) =>
                {
                    var position = GetPoint(i).position;
                    return guiState.DistanceToCircle(position, guiState.GetHandleSize(position) * 10f);
                },
                position = (i) => { return GetPoint(i).position; },
                forward = (i) => { return GetForward(); },
                up = (i) => { return GetUp(); },
                right = (i) => { return GetRight(); },
                onRepaint = DrawPoint
            };

            var m_EdgeControl = new GenericControl("Edge")
            {
                count = GetEdgeCount,
                distance = DistanceToEdge,
                position = (i) => { return GetPoint(i).position; },
                forward = (i) => { return GetForward(); },
                up = (i) => { return GetUp(); },
                right = (i) => { return GetRight(); },
                onRepaint = DrawEdge
            };
            m_EdgeControl.onEndLayout = (guiState) => { controller.AddClosestPath(m_EdgeControl.layoutData.distance); };

            var m_LeftTangentControl = new GenericControl("LeftTangent")
            {
                count = () =>
                {
                    if (GetShapeType() != ShapeType.Spline)
                        return 0;

                    return GetPointCount();
                },
                distance = (guiState, i) =>
                {
                    if (linearTangentIsZero && GetPoint(i).tangentMode == TangentMode.Linear)
                        return float.MaxValue;

                    if (!IsSelected(i) || IsOpenEnded() && i == 0)
                        return float.MaxValue;

                    var position = GetLeftTangent(i);
                    return guiState.DistanceToCircle(position, guiState.GetHandleSize(position) * 10f);
                },
                position = (i) => { return GetLeftTangent(i); },
                forward = (i) => { return GetForward(); },
                up = (i) => { return GetUp(); },
                right = (i) => { return GetRight(); },
                onRepaint = (guiState, control, i) =>
                {
                    if (!IsSelected(i) || IsOpenEnded() && i == 0)
                        return;

                    var point = GetPoint(i);

                    if (linearTangentIsZero && point.tangentMode == TangentMode.Linear)
                        return;
                    
                    var position = point.position;
                    var leftTangent = GetLeftTangent(i);
                    
                    drawer.DrawTangent(position, leftTangent);
                }
            };

            var m_RightTangentControl = new GenericControl("RightTangent")
            {
                count = () =>
                {
                    if (GetShapeType() != ShapeType.Spline)
                        return 0;
                        
                    return GetPointCount();
                },
                distance = (guiState, i) =>
                {
                    if (linearTangentIsZero && GetPoint(i).tangentMode == TangentMode.Linear)
                        return float.MaxValue;

                    if (!IsSelected(i) || IsOpenEnded() && i == GetPointCount()-1)
                        return float.MaxValue;

                    var position = GetRightTangent(i);
                    return guiState.DistanceToCircle(position, guiState.GetHandleSize(position) * 10f);
                },
                position = (i) => { return GetRightTangent(i); },
                forward = (i) => { return GetForward(); },
                up = (i) => { return GetUp(); },
                right = (i) => { return GetRight(); },
                onRepaint = (guiState, control, i) =>
                {
                    if (!IsSelected(i) || IsOpenEnded() && i == GetPointCount()-1)
                        return;
                    
                    var point = GetPoint(i);

                    if (linearTangentIsZero && point.tangentMode == TangentMode.Linear)
                        return;
                    
                    var position = point.position;
                    var rightTangent = GetRightTangent(i);

                    drawer.DrawTangent(position, rightTangent);
                }
            };

            var m_CreatePointAction = new CreatePointAction(m_PointControl, m_EdgeControl)
            {
                enable = (guiState, action) => !IsAltDown(guiState) && !guiState.isActionKeyDown && controller.closestEditablePath == controller.editablePath,
                enableRepaint = (guiState, action) => EnableCreatePointRepaint(guiState, m_PointControl, m_LeftTangentControl, m_RightTangentControl),
                repaintOnMouseMove = (guiState, action) => true,
                guiToWorld = GUIToWorld,
                onCreatePoint = (index, position) =>
                {
                    controller.RegisterUndo("Create Point");
                    controller.CreatePoint(index, position);
                },
                onPreRepaint = (guiState, action) =>
                {
                    if (GetPointCount() > 0)
                    {
                        var position = ClosestPointInEdge(guiState, guiState.mousePosition, m_EdgeControl.layoutData.index);
                        drawer.DrawCreatePointPreview(position);
                    }
                }
            };

            Action<IGUIState> removePoints = (guiState) =>
            {
                controller.RegisterUndo("Remove Point");
                controller.RemoveSelectedPoints();
                guiState.changed = true;
            };

            var m_RemovePointAction1 = new CommandAction(kDeleteCommandName)
            {
                enable = (guiState, action) => { return GetSelectedPointCount() > 0; },
                onCommand = removePoints
            };

            var m_RemovePointAction2 = new CommandAction(kSoftDeleteCommandName)
            {
                enable = (guiState, action) => { return GetSelectedPointCount() > 0; },
                onCommand = removePoints
            };

            var dragged = false;
            var m_MovePointAction = new SliderAction(m_PointControl)
            {
                enable = (guiState, action) => !IsAltDown(guiState),
                onClick = (guiState, control) =>
                {
                    dragged = false;
                    var index = control.layoutData.index;

                    if (!IsSelected(index))
                    {
                        controller.RegisterUndo("Selection");

                        if (!guiState.isActionKeyDown)
                            controller.ClearSelection();

                        controller.SelectPoint(index, true);
                        guiState.changed = true;
                    }
                },
                onSliderChanged = (guiState, control, position) =>
                {
                    var index = control.hotLayoutData.index;
                    var delta = SnapIfNeeded(position) - GetPoint(index).position;

                    if (!dragged)
                    {
                        controller.RegisterUndo("Move Point");
                        dragged = true;
                    }
                    
                    controller.MoveSelectedPoints(delta);
                }
            };

            var m_MoveEdgeAction = new SliderAction(m_EdgeControl)
            {
                enable = (guiState, action) => !IsAltDown(guiState) && guiState.isActionKeyDown,
                onSliderBegin = (guiState, control, position) =>
                {
                    dragged = false;
                    
                },
                onSliderChanged = (guiState, control, position) =>
                {
                    var index = control.hotLayoutData.index;
                    var delta = position -  GetPoint(index).position;
                    
                    if (!dragged)
                    {
                        controller.RegisterUndo("Move Edge");
                        dragged = true;
                    }

                    controller.MoveEdge(index, delta);
                }
            };

            var cachedRightTangent = Vector3.zero;
            var cachedLeftTangent = Vector3.zero;
            var cachedTangentMode = TangentMode.Linear;
            
            var m_MoveLeftTangentAction = new SliderAction(m_LeftTangentControl)
            {
                enable = (guiState, action) => !IsAltDown(guiState),
                onSliderBegin = (guiState, control, position) =>
                {
                    dragged = false;
                    var point = GetPoint(control.hotLayoutData.index);
                    cachedRightTangent = point.rightTangent;
                    cachedTangentMode = point.tangentMode;
                },
                onSliderChanged = (guiState, control, position) =>
                {
                    var index = control.hotLayoutData.index;
                    var setToLinear = m_PointControl.distance(guiState, index) <= DefaultControl.kPickDistance;

                    if (!dragged)
                    {
                        controller.RegisterUndo("Move Tangent");
                        dragged = true;
                    }

                    position = SnapIfNeeded(position);
                    controller.SetLeftTangent(index, position, setToLinear, guiState.isShiftDown, cachedRightTangent, cachedTangentMode);
                    
                }
            };

            var m_MoveRightTangentAction = new SliderAction(m_RightTangentControl)
            {
                enable = (guiState, action) => !IsAltDown(guiState),
                onSliderBegin = (guiState, control, position) =>
                {
                    dragged = false;
                    var point = GetPoint(control.hotLayoutData.index);
                    cachedLeftTangent = point.leftTangent;
                    cachedTangentMode = point.tangentMode;
                },
                onSliderChanged = (guiState, control, position) =>
                {
                    var index = control.hotLayoutData.index;
                    var setToLinear = m_PointControl.distance(guiState, index) <= DefaultControl.kPickDistance;

                    if (!dragged)
                    {
                        controller.RegisterUndo("Move Tangent");
                        dragged = true;
                    }

                    position = SnapIfNeeded(position);
                    controller.SetRightTangent(index, position, setToLinear, guiState.isShiftDown, cachedLeftTangent, cachedTangentMode);
                }
            };

            m_GUISystem.AddControl(m_EdgeControl);
            m_GUISystem.AddControl(m_PointControl);
            m_GUISystem.AddControl(m_LeftTangentControl);
            m_GUISystem.AddControl(m_RightTangentControl);
            m_GUISystem.AddAction(m_CreatePointAction);
            m_GUISystem.AddAction(m_RemovePointAction1);
            m_GUISystem.AddAction(m_RemovePointAction2);
            m_GUISystem.AddAction(m_MovePointAction);
            m_GUISystem.AddAction(m_MoveEdgeAction);
            m_GUISystem.AddAction(m_MoveLeftTangentAction);
            m_GUISystem.AddAction(m_MoveRightTangentAction);
        }

        public void OnGUI()
        {
            m_GUISystem.OnGUI();
        }

        private bool IsAltDown(IGUIState guiState)
        {
            return guiState.hotControl == 0 && guiState.isAltDown;
        }

        private ControlPoint GetPoint(int index)
        {
            return controller.editablePath.GetPoint(index);
        }

        private int GetPointCount()
        {
            return controller.editablePath.pointCount;
        }

        private int GetEdgeCount()
        {
            if (controller.editablePath.isOpenEnded)
                return controller.editablePath.pointCount - 1;

            return controller.editablePath.pointCount;
        }

        private int GetSelectedPointCount()
        {
            return controller.editablePath.selection.Count;
        }

        private bool IsSelected(int index)
        {
            return controller.editablePath.selection.Contains(index);
        }

        private Vector3 GetForward()
        {
            return controller.editablePath.forward;
        }

        private Vector3 GetUp()
        {
            return controller.editablePath.up;
        }

        private Vector3 GetRight()
        {
            return controller.editablePath.right;
        }

        private Matrix4x4 GetLocalToWorldMatrix()
        {
            return controller.editablePath.localToWorldMatrix;
        }

        private ShapeType GetShapeType()
        {
            return controller.editablePath.shapeType;
        }

        private bool IsOpenEnded()
        {
            return controller.editablePath.isOpenEnded;
        }

        private Vector3 GetLeftTangent(int index)
        {
            if (linearTangentIsZero)
                return GetPoint(index).leftTangent;
            
            return controller.editablePath.CalculateLeftTangent(index);
        }

        private Vector3 GetRightTangent(int index)
        {
            if (linearTangentIsZero)
                return GetPoint(index).rightTangent;

            return controller.editablePath.CalculateRightTangent(index);
        }

        private int NextIndex(int index)
        {
            return EditablePathUtility.Mod(index + 1, GetPointCount());
        }

        private ControlPoint NextControlPoint(int index)
        {
            return GetPoint(NextIndex(index));
        }

        private int PrevIndex(int index)
        {
            return EditablePathUtility.Mod(index - 1, GetPointCount());
        }

        private ControlPoint PrevControlPoint(int index)
        {
            return GetPoint(PrevIndex(index));
        }

        private Vector3 ClosestPointInEdge(IGUIState guiState, Vector2 mousePosition, int index)
        {
            if (GetShapeType() == ShapeType.Polygon)
            {
                var p0 = GetPoint(index).position;
                var p1 = NextControlPoint(index).position;
                var mouseWorldPosition = GUIToWorld(guiState, mousePosition);

                var dir1 = (mouseWorldPosition - p0);
                var dir2 = (p1 - p0);
                
                return Mathf.Clamp01(Vector3.Dot(dir1, dir2.normalized) / dir2.magnitude) * dir2 + p0;
            }
            else if (GetShapeType() == ShapeType.Spline)
            {
                var nextIndex = NextIndex(index);
                float t;
                return BezierUtility.ClosestPointOnCurve(
                    GUIToWorld(guiState, mousePosition),
                    GetPoint(index).position,
                    GetPoint(nextIndex).position,
                    GetRightTangent(index),
                    GetLeftTangent(nextIndex),
                    out t);
            }

            return Vector3.zero;
        }

        private float DistanceToEdge(IGUIState guiState, int index)
        {
            if (GetShapeType() == ShapeType.Polygon)
            {
                return guiState.DistanceToSegment(GetPoint(index).position, NextControlPoint(index).position);
            }
            else if (GetShapeType() == ShapeType.Spline)
            {
                var closestPoint = ClosestPointInEdge(guiState, guiState.mousePosition, index);
                var closestPoint2 = HandleUtility.WorldToGUIPoint(closestPoint);

                return (closestPoint2 - guiState.mousePosition).magnitude;
            }

            return float.MaxValue;
        }

        private Vector3 GUIToWorld(IGUIState guiState, Vector2 position)
        {
            return guiState.GUIToWorld(position, GetForward(), GetLocalToWorldMatrix().MultiplyPoint3x4(Vector3.zero));
        }

        private void DrawPoint(IGUIState guiState, Control control, int index)
        {
            var position = GetPoint(index).position;

            if (guiState.hotControl == control.actionID && control.hotLayoutData.index == index || IsSelected(index))
                drawer.DrawPointSelected(position);
            else if (guiState.hotControl == 0 && guiState.nearestControl == control.ID && !IsAltDown(guiState) && control.layoutData.index == index)
                drawer.DrawPointHovered(position);
            else
                drawer.DrawPoint(position);
        }

        private void DrawEdge(IGUIState guiState, Control control, int index)
        {
            if (GetShapeType() == ShapeType.Polygon)
            {
                var nextIndex = NextIndex(index);
                var color = Color.white;

                if(guiState.nearestControl == control.ID && control.layoutData.index == index && guiState.hotControl == 0 && !IsAltDown(guiState))
                    color = Color.yellow;
                
                drawer.DrawLine(GetPoint(index).position, GetPoint(nextIndex).position, 5f, color);
            }
            else if (GetShapeType() == ShapeType.Spline)
            {
                var nextIndex = NextIndex(index);
                var color = Color.white;

                if(guiState.nearestControl == control.ID && control.layoutData.index == index && guiState.hotControl == 0 && !IsAltDown(guiState))
                    color = Color.yellow;
                
                drawer.DrawBezier(
                    GetPoint(index).position,
                    GetRightTangent(index),
                    GetLeftTangent(nextIndex),
                    GetPoint(nextIndex).position,
                    5f,
                    color);
            }
        }

        private bool EnableCreatePointRepaint(IGUIState guiState, Control pointControl, Control leftTangentControl, Control rightTangentControl)
        {
            return guiState.nearestControl != pointControl.ID &&
                    guiState.hotControl == 0  &&
                    (guiState.nearestControl != leftTangentControl.ID) &&
                    (guiState.nearestControl != rightTangentControl.ID);
        }

        private Vector3 SnapIfNeeded(Vector3 position)
        {
            if (!controller.enableSnapping || controller.snapping == null)
                return position;
            
            var guiPosition = HandleUtility.WorldToGUIPoint(position);
            var snappedGuiPosition = HandleUtility.WorldToGUIPoint(controller.snapping.Snap(position));
            var sqrDistance = (guiPosition - snappedGuiPosition).sqrMagnitude;

            if (sqrDistance < kSnappingDistance * kSnappingDistance)
                position = controller.snapping.Snap(position);
            
            return position;
        }
    }
}
