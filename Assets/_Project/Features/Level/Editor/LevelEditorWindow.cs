using StackAttack.Core;
using UnityEditor;
using UnityEngine;

namespace StackAttack.Level
{
    public class LevelEditorWindow : EditorWindow
    {
        private const float PanelWidth = 320f;

        private static readonly Color GridLine = new Color(0f, 0f, 0f, 0.35f);
        private static readonly Color CellEmpty = new Color(0.22f, 0.22f, 0.24f, 1f);
        private static readonly Color CellOccupied = new Color(0.75f, 0.2f, 0.2f, 0.45f);
        private static readonly Color CellConflict = new Color(0.95f, 0.35f, 0.1f, 0.75f);
        private static readonly Color CellSelected = new Color(1f, 0.85f, 0.2f, 0.9f);
        private static readonly Color DropValid = new Color(0.35f, 0.9f, 0.4f, 0.9f);
        private static readonly Color DropInvalid = new Color(0.95f, 0.2f, 0.2f, 0.9f);
        private static readonly Color CellMissingType = new Color(0.95f, 0.75f, 0.1f, 0.9f);

        private LevelConfig _config;
        private SerializedObject _serialized;
        private SerializedProperty _levelLength;
        private SerializedProperty _scrollSpeed;
        private SerializedProperty _entries;

        private StackTypeConfig _brushType;
        private int _brushHp = 12;
        private int _laneCount = 5;
        private float _cellSize = 44f;
        private int _selected = -1;
        private Vector2 _gridScroll;

        private int _dragIndex = -1;
        private int _dragLane;
        private int _dragRow;
        private bool _dragInside;

        [MenuItem("StackAttack/Level Editor")]
        private static void Open()
        {
            GetWindow<LevelEditorWindow>("Level Editor").Show();
        }

        private void OnSelectionChange()
        {
            LevelConfig selected = Selection.activeObject as LevelConfig;

            if (selected != null && selected != _config)
            {
                Bind(selected);
                Repaint();
            }
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (_config == null)
            {
                EditorGUILayout.HelpBox("Assign a Level Config to start editing.", MessageType.Info);
                return;
            }

            _serialized.Update();

            EditorGUILayout.BeginHorizontal();
            DrawGrid();
            DrawPanel();
            EditorGUILayout.EndHorizontal();

            if (_serialized.ApplyModifiedProperties())
                Repaint();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            LevelConfig picked = (LevelConfig)EditorGUILayout.ObjectField(_config, typeof(LevelConfig), false, GUILayout.Width(180f));

            if (picked != _config)
                Bind(picked);

            GUILayout.Space(12f);
            GUILayout.Label("Brush", EditorStyles.miniLabel, GUILayout.Width(38f));
            _brushType = (StackTypeConfig)EditorGUILayout.ObjectField(_brushType, typeof(StackTypeConfig), false, GUILayout.Width(140f));

            GUILayout.Label("HP", EditorStyles.miniLabel, GUILayout.Width(20f));
            _brushHp = EditorGUILayout.IntField(_brushHp, GUILayout.Width(40f));

            GUILayout.Space(12f);
            GUILayout.Label("Lanes", EditorStyles.miniLabel, GUILayout.Width(38f));
            _laneCount = Mathf.Clamp(EditorGUILayout.IntField(_laneCount, GUILayout.Width(30f)), 1, 15);

            GUILayout.Label("Zoom", EditorStyles.miniLabel, GUILayout.Width(38f));
            _cellSize = GUILayout.HorizontalSlider(_cellSize, 20f, 90f, GUILayout.Width(90f));

            if (_brushType == null)
                GUILayout.Label("Brush needs a stack type", EditorStyles.miniLabel);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void Bind(LevelConfig config)
        {
            _config = config;
            _selected = -1;
            _serialized = null;

            if (_config == null)
                return;

            _serialized = new SerializedObject(_config);
            _levelLength = _serialized.FindProperty("levelLength");
            _scrollSpeed = _serialized.FindProperty("scrollSpeed");
            _entries = _serialized.FindProperty("entries");
        }

        private void DrawGrid()
        {
            int rows = Mathf.Max(Mathf.CeilToInt(_config.LevelLength), 1);
            float width = _laneCount * _cellSize;
            float height = rows * _cellSize;

            _gridScroll = EditorGUILayout.BeginScrollView(_gridScroll, GUILayout.ExpandWidth(true));

            Rect canvas = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

            for (int row = 0; row < rows; row++)
            {
                for (int lane = 0; lane < _laneCount; lane++)
                {
                    Rect cell = CellRect(canvas, lane, row, rows);
                    DrawCell(cell, lane, row);
                }
            }

            DrawDropPreview(canvas, rows);
            HandleGridInput(canvas, rows);

            EditorGUILayout.EndScrollView();
        }

        private Rect CellRect(Rect canvas, int lane, int row, int rows)
        {
            return new Rect(
                canvas.x + lane * _cellSize,
                canvas.y + (rows - 1 - row) * _cellSize,
                _cellSize,
                _cellSize);
        }

        private void DrawCell(Rect cell, int lane, int row)
        {
            EditorGUI.DrawRect(cell, CellEmpty);
            EditorGUI.DrawRect(new Rect(cell.x, cell.y, cell.width, 1f), GridLine);
            EditorGUI.DrawRect(new Rect(cell.x, cell.y, 1f, cell.height), GridLine);

            int occupants = CountOccupants(lane, row);

            if (occupants == 1)
                EditorGUI.DrawRect(cell, CellOccupied);
            else if (occupants > 1)
                EditorGUI.DrawRect(cell, CellConflict);

            int anchor = AnchorAt(lane, row);

            if (anchor < 0)
                return;

            StackGroupEntry entry = _config.Entries[anchor];

            Rect inner = new Rect(cell.x + 4f, cell.y + 4f, cell.width - 8f, cell.height - 8f);
            EditorGUI.DrawRect(inner, entry.stackType != null ? entry.stackType.PlateColor : CellMissingType);

            if (anchor == _selected)
            {
                EditorGUI.DrawRect(new Rect(cell.x, cell.y, cell.width, 2f), CellSelected);
                EditorGUI.DrawRect(new Rect(cell.x, cell.yMax - 2f, cell.width, 2f), CellSelected);
                EditorGUI.DrawRect(new Rect(cell.x, cell.y, 2f, cell.height), CellSelected);
                EditorGUI.DrawRect(new Rect(cell.xMax - 2f, cell.y, 2f, cell.height), CellSelected);
            }

            string caption = entry.stackType != null ? StackGroupGeometry.PlateCount(entry).ToString() : "!";

            GUI.Label(cell, caption, CenteredLabel());
        }

        private static GUIStyle CenteredLabel()
        {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);

            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;

            return style;
        }

        private float LaneX(int lane)
        {
            return lane - (_laneCount - 1) * 0.5f;
        }

        private int CountOccupants(int lane, int row)
        {
            int count = 0;

            for (int i = 0; i < _config.Entries.Count; i++)
            {
                if (Occupies(_config.Entries[i], lane, row))
                    count++;
            }

            return count;
        }

        private int AnchorAt(int lane, int row)
        {
            for (int i = 0; i < _config.Entries.Count; i++)
            {
                StackGroupEntry entry = _config.Entries[i];

                if (Mathf.Abs(entry.xPosition - LaneX(lane)) < 0.5f && Mathf.FloorToInt(entry.distance) == row)
                    return i;
            }

            return -1;
        }

        private bool Occupies(StackGroupEntry entry, int lane, int row)
        {
            if (entry.stackType == null)
                return false;

            float height = Mathf.Max(StackGroupGeometry.StackHeight(entry), 0.25f);
            float cellX = LaneX(lane);

            if (entry.layout == GroupLayout.Ring)
            {
                float reach = StackGroupGeometry.HalfWidth(entry) + height * 0.5f;
                float dx = cellX - entry.xPosition;
                float dy = row + 0.5f - (entry.distance + height * 0.5f);

                return dx * dx + dy * dy <= reach * reach;
            }

            bool rowsOverlap = entry.distance < row + 1f && entry.distance + height > row;

            if (!rowsOverlap)
                return false;

            // Every group descends at the same speed, so a horizontally moving one keeps
            // its rows to itself forever while sweeping the full width of the playfield.
            if (entry.motion == GroupMotion.Horizontal)
                return true;

            float halfWidth = StackGroupGeometry.HalfWidth(entry);

            return entry.xPosition - halfWidth < cellX + 0.5f && entry.xPosition + halfWidth > cellX - 0.5f;
        }

        private void DrawDropPreview(Rect canvas, int rows)
        {
            if (_dragIndex < 0 || !_dragInside)
                return;

            Rect target = CellRect(canvas, _dragLane, _dragRow, rows);
            Color color = CanDrop(_dragIndex, _dragLane, _dragRow) ? DropValid : DropInvalid;

            EditorGUI.DrawRect(new Rect(target.x, target.y, target.width, 3f), color);
            EditorGUI.DrawRect(new Rect(target.x, target.yMax - 3f, target.width, 3f), color);
            EditorGUI.DrawRect(new Rect(target.x, target.y, 3f, target.height), color);
            EditorGUI.DrawRect(new Rect(target.xMax - 3f, target.y, 3f, target.height), color);
        }

        private void HandleGridInput(Rect canvas, int rows)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            Event current = Event.current;

            switch (current.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    if (current.button != 0 || !canvas.Contains(current.mousePosition))
                        return;

                    ResolveCell(canvas, rows, current.mousePosition);

                    int anchor = AnchorAt(_dragLane, _dragRow);

                    if (anchor >= 0)
                    {
                        _selected = anchor;
                        _dragIndex = anchor;
                        GUIUtility.hotControl = controlId;
                    }
                    else
                    {
                        CreateEntry(_dragLane, _dragRow);
                    }

                    current.Use();
                    Repaint();
                    return;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != controlId)
                        return;

                    ResolveCell(canvas, rows, current.mousePosition);
                    current.Use();
                    Repaint();
                    return;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl != controlId)
                        return;

                    GUIUtility.hotControl = 0;

                    // Dropping anywhere invalid simply keeps the entry where it was,
                    // because nothing is written until the drop is accepted.
                    if (_dragIndex >= 0 && _dragInside && CanDrop(_dragIndex, _dragLane, _dragRow))
                        MoveEntry(_dragIndex, _dragLane, _dragRow);

                    _dragIndex = -1;
                    current.Use();
                    Repaint();
                    return;
            }
        }

        private void ResolveCell(Rect canvas, int rows, Vector2 mouse)
        {
            int lane = Mathf.FloorToInt((mouse.x - canvas.x) / _cellSize);
            int fromTop = Mathf.FloorToInt((mouse.y - canvas.y) / _cellSize);
            int row = rows - 1 - fromTop;

            _dragInside = lane >= 0 && lane < _laneCount && row >= 0 && row < rows;
            _dragLane = Mathf.Clamp(lane, 0, _laneCount - 1);
            _dragRow = Mathf.Clamp(row, 0, rows - 1);
        }

        private bool CanDrop(int index, int lane, int row)
        {
            int anchor = AnchorAt(lane, row);

            return anchor < 0 || anchor == index;
        }

        private void MoveEntry(int index, int lane, int row)
        {
            SerializedProperty entry = _entries.GetArrayElementAtIndex(index);

            entry.FindPropertyRelative("distance").floatValue = row;
            entry.FindPropertyRelative("xPosition").floatValue = LaneX(lane);

            _serialized.ApplyModifiedProperties();
        }

        private void CreateEntry(int lane, int row)
        {
            // An entry without a type throws the moment it spawns, so the brush has to
            // be filled before anything can be placed.
            if (_brushType == null)
            {
                Debug.LogWarning("Pick a Stack Type in the Brush field before placing entries.");
                return;
            }

            int index = _entries.arraySize;
            _entries.InsertArrayElementAtIndex(index);

            SerializedProperty entry = _entries.GetArrayElementAtIndex(index);

            entry.FindPropertyRelative("distance").floatValue = row;
            entry.FindPropertyRelative("xPosition").floatValue = LaneX(lane);
            entry.FindPropertyRelative("stackType").objectReferenceValue = _brushType;
            entry.FindPropertyRelative("hp").intValue = _brushHp;
            entry.FindPropertyRelative("layout").enumValueIndex = (int)GroupLayout.Single;
            entry.FindPropertyRelative("count").intValue = 1;
            entry.FindPropertyRelative("spacing").floatValue = 1f;
            entry.FindPropertyRelative("radius").floatValue = 1f;
            entry.FindPropertyRelative("motion").enumValueIndex = (int)GroupMotion.Static;
            entry.FindPropertyRelative("motionSpeed").floatValue = 0f;

            _serialized.ApplyModifiedProperties();
            _selected = index;
        }

        private void DrawPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(PanelWidth));

            EditorGUILayout.LabelField("Level", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_levelLength);
            EditorGUILayout.PropertyField(_scrollSpeed);
            EditorGUILayout.LabelField("Entries", _entries.arraySize.ToString());

            EditorGUILayout.Space();

            if (_selected < 0 || _selected >= _entries.arraySize)
            {
                EditorGUILayout.HelpBox("Click an empty cell to place a stack, or an existing one to edit it.", MessageType.None);
                EditorGUILayout.EndVertical();
                return;
            }

            EditorGUILayout.LabelField("Selected Entry " + _selected, EditorStyles.boldLabel);

            SerializedProperty entry = _entries.GetArrayElementAtIndex(_selected);
            SerializedProperty layout = entry.FindPropertyRelative("layout");
            SerializedProperty motion = entry.FindPropertyRelative("motion");

            EditorGUILayout.PropertyField(entry.FindPropertyRelative("distance"));
            EditorGUILayout.PropertyField(entry.FindPropertyRelative("xPosition"));
            EditorGUILayout.PropertyField(entry.FindPropertyRelative("stackType"));
            EditorGUILayout.PropertyField(entry.FindPropertyRelative("hp"));
            EditorGUILayout.PropertyField(layout);

            GroupLayout layoutValue = (GroupLayout)layout.enumValueIndex;

            if (layoutValue == GroupLayout.Row)
            {
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("count"));
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("spacing"));
            }
            else if (layoutValue == GroupLayout.Ring)
            {
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("count"));
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("radius"));
            }

            EditorGUILayout.PropertyField(motion);

            if ((GroupMotion)motion.enumValueIndex != GroupMotion.Static)
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("motionSpeed"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Plates", StackGroupGeometry.PlateCount(_config.Entries[_selected]).ToString());
            EditorGUILayout.LabelField("Half Width", StackGroupGeometry.HalfWidth(_config.Entries[_selected]).ToString("0.00"));

            EditorGUILayout.Space();

            if (GUILayout.Button("Delete Entry"))
            {
                _entries.DeleteArrayElementAtIndex(_selected);
                _serialized.ApplyModifiedProperties();
                _selected = -1;
            }

            EditorGUILayout.EndVertical();
        }
    }
}
