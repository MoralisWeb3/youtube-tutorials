using UnityEngine;

namespace UnityEditor.Tilemaps
{
    internal class GridPaletteAddPopup : EditorWindow
    {
        static class Styles
        {
            public static readonly GUIContent nameLabel = EditorGUIUtility.TrTextContent("Name");
            public static readonly GUIContent ok = EditorGUIUtility.TrTextContent("Create");
            public static readonly GUIContent cancel = EditorGUIUtility.TrTextContent("Cancel");
            public static readonly GUIContent header = EditorGUIUtility.TrTextContent("Create New Palette");
            public static readonly GUIContent gridLabel = EditorGUIUtility.TrTextContent("Grid");
            public static readonly GUIContent sizeLabel = EditorGUIUtility.TrTextContent("Cell Size");
            public static readonly GUIContent hexagonLabel = EditorGUIUtility.TrTextContent("Hexagon Type");
            public static readonly GUIContent[] hexagonSwizzleTypeLabel =
            {
                EditorGUIUtility.TrTextContent("Point Top"),
                EditorGUIUtility.TrTextContent("Flat Top"),
            };
            public static readonly GridLayout.CellSwizzle[] hexagonSwizzleTypeValue =
            {
                GridLayout.CellSwizzle.XYZ,
                GridLayout.CellSwizzle.YXZ,
            };

            public static readonly GUIContent transparencySortModeLabel =
                EditorGUIUtility.TrTextContent("Sort Mode");
            public static readonly GUIContent transparencySortAxisLabel =
                EditorGUIUtility.TrTextContent("Sort Axis");
        }

        private static long s_LastClosedTime;
        private string m_Name = "New Palette";
        private static GridPaletteAddPopup s_Instance;
        private GridPaintPaletteWindow m_Owner;
        private GridLayout.CellLayout m_Layout;
        private int m_HexagonLayout;
        private GridPalette.CellSizing m_CellSizing;
        private Vector3 m_CellSize;
        private TransparencySortMode m_TransparencySortMode;
        private Vector3 m_TransparencySortAxis = new Vector3(0f, 0f, 1f);

        void Init(Rect buttonRect, GridPaintPaletteWindow owner)
        {
            m_Owner = owner;
            m_CellSize = new Vector3(1, 1, 0);
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            ShowAsDropDown(buttonRect, new Vector2(312, 185));
        }

        internal void OnGUI()
        {
            GUI.Label(new Rect(0, 0, position.width, position.height), GUIContent.none, "grey_border");
            GUILayout.Space(3);

            GUILayout.Label(Styles.header, EditorStyles.boldLabel);
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            GUILayout.Label(Styles.nameLabel, GUILayout.Width(90f));
            m_Name = EditorGUILayout.TextField(m_Name);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Styles.gridLabel, GUILayout.Width(90f));
            EditorGUI.BeginChangeCheck();
            var newLayout = (GridLayout.CellLayout)EditorGUILayout.EnumPopup(m_Layout);
            if (EditorGUI.EndChangeCheck())
            {
                // Set useful user settings for certain layouts
                switch (newLayout)
                {
                    case GridLayout.CellLayout.Rectangle:
                    case GridLayout.CellLayout.Hexagon:
                    {
                        m_CellSizing = GridPalette.CellSizing.Automatic;
                        m_CellSize = new Vector3(1, 1, 0);
                        break;
                    }
                    case GridLayout.CellLayout.Isometric:
                    {
                        m_CellSizing = GridPalette.CellSizing.Manual;
                        m_CellSize = new Vector3(1, 0.5f, 1);
                        break;
                    }
                    case GridLayout.CellLayout.IsometricZAsY:
                    {
                        m_CellSizing = GridPalette.CellSizing.Manual;
                        m_CellSize = new Vector3(1, 0.5f, 1);
                        m_TransparencySortMode = TransparencySortMode.CustomAxis;
                        m_TransparencySortAxis = new Vector3(0f, 1f, -0.25f);
                        break;
                    }
                }
                m_Layout = newLayout;
            }
            GUILayout.EndHorizontal();

            if (m_Layout == GridLayout.CellLayout.Hexagon)
            {
                GUILayout.BeginHorizontal();
                float oldLabelWidth = UnityEditor.EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 94;
                m_HexagonLayout = EditorGUILayout.Popup(Styles.hexagonLabel, m_HexagonLayout, Styles.hexagonSwizzleTypeLabel);
                EditorGUIUtility.labelWidth = oldLabelWidth;
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(Styles.sizeLabel, GUILayout.Width(90f));
            m_CellSizing = (GridPalette.CellSizing)EditorGUILayout.EnumPopup(m_CellSizing);
            GUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(m_CellSizing == GridPalette.CellSizing.Automatic))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(GUIContent.none, GUILayout.Width(90f));
                m_CellSize = EditorGUILayout.Vector3Field(GUIContent.none, m_CellSize);
                GUILayout.EndHorizontal();
            }
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Styles.transparencySortModeLabel, GUILayout.Width(90f));
            m_TransparencySortMode = (TransparencySortMode)EditorGUILayout.EnumPopup(m_TransparencySortMode);
            GUILayout.EndHorizontal();
            using (new EditorGUI.DisabledScope(m_TransparencySortMode != TransparencySortMode.CustomAxis))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(Styles.transparencySortAxisLabel, GUILayout.Width(90f));
                m_TransparencySortAxis = EditorGUILayout.Vector3Field("", m_TransparencySortAxis);
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();

            // Cancel, Ok
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button(Styles.cancel))
            {
                Close();
            }

            using (new EditorGUI.DisabledScope(!Utils.Paths.IsValidAssetPath(m_Name)))
            {
                if (GUILayout.Button(Styles.ok))
                {
                    s_LastClosedTime = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;

                    // case 1077362: Close window to prevent overlap with OS folder window when saving new palette asset
                    Close();

                    var swizzle = GridLayout.CellSwizzle.XYZ;
                    if (m_Layout == GridLayout.CellLayout.Hexagon)
                        swizzle = Styles.hexagonSwizzleTypeValue[m_HexagonLayout];

                    GameObject go = GridPaletteUtility.CreateNewPaletteAtCurrentFolder(m_Name, m_Layout, m_CellSizing, m_CellSize
                        , swizzle, m_TransparencySortMode, m_TransparencySortAxis);
                    if (go != null)
                    {
                        m_Owner.palette = go;
                        m_Owner.Repaint();
                    }

                    GUIUtility.ExitGUI();
                }
            }

            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }

        internal static bool ShowAtPosition(Rect buttonRect, GridPaintPaletteWindow owner)
        {
            // We could not use realtimeSinceStartUp since it is set to 0 when entering/exitting playmode, we assume an increasing time when comparing time.
            long nowMilliSeconds = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
            bool justClosed = nowMilliSeconds < s_LastClosedTime + 50;
            if (!justClosed)
            {
                Event.current.Use();
                if (s_Instance == null)
                    s_Instance = ScriptableObject.CreateInstance<GridPaletteAddPopup>();

                s_Instance.Init(buttonRect, owner);
                return true;
            }
            return false;
        }
    }
}

// namespace
