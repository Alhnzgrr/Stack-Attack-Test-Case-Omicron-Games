using StackAttack.Core;
using UnityEditor;
using UnityEngine;

namespace StackAttack.Level
{
    public class LevelEditorWindow : EditorWindow
    {
        private const float PanelWidth = 320f;
        private const float GutterWidth = 44f;
        private const int MajorStep = 5;

        private static readonly Color Backdrop = new Color(0.16f, 0.16f, 0.18f, 1f);
        private static readonly Color MinorLine = new Color(1f, 1f, 1f, 0.06f);
        private static readonly Color MajorLine = new Color(1f, 1f, 1f, 0.18f);
        private static readonly Color CentreLine = new Color(1f, 1f, 1f, 0.12f);
        private static readonly Color EdgeLine = new Color(0.35f, 0.75f, 1f, 0.55f);
        private static readonly Color GroupFill = new Color(1f, 1f, 1f, 0.05f);
        private static readonly Color SweepFill = new Color(0.95f, 0.75f, 0.1f, 0.07f);
        private static readonly Color MissingType = new Color(0.95f, 0.75f, 0.1f, 0.9f);
        private static readonly Color GroupOutline = new Color(0f, 0f, 0f, 0.55f);
        private static readonly Color ConflictOutline = new Color(0.95f, 0.3f, 0.15f, 1f);
        private static readonly Color SelectedOutline = new Color(1f, 0.85f, 0.2f, 1f);

        private static readonly float[] SnapSteps = { 0f, 0.1f, 0.25f, 0.5f, 1f };
        private static readonly string[] SnapLabels = { "Off", "0.1", "0.25", "0.5", "1" };

        private LevelConfig _config;
        private PlayfieldConfig _playfield;
        private SerializedObject _serialized;
        private SerializedProperty _levelLength;
        private SerializedProperty _scrollSpeed;
        private SerializedProperty _upgradeCostScale;
        private SerializedProperty _entries;

        private StackTypeConfig _brushType;
        private int _brushHp = 12;
        private int _snapIndex = 3;
        private float _zoom = 44f;
        private int _selected = -1;
        private Vector2 _scroll;

        private int _dragIndex = -1;
        private Vector2 _dragGrab;
        private Vector2 _dragAnchor;
        private Vector2 _dragPosition;
        private bool _dragMoved;

        private Rect _field;
        private float _halfWidth;
        private GUIStyle _captionStyle;
        private GUIStyle _tickStyle;

        [MenuItem("StackAttack/Level Editor")]
        private static void Open()
        {
            GetWindow<LevelEditorWindow>("Level Editor").Show();
        }

        private void OnEnable()
        {
            _playfield = FindPlayfield();
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

            if (_playfield == null)
            {
                EditorGUILayout.HelpBox("Assign a Playfield Config. The field is drawn to its width.", MessageType.Warning);
                return;
            }

            // A domain reload wipes the SerializedObject but keeps the LevelConfig,
            // so entering play mode leaves the window holding one without the other.
            if (_serialized == null || _serialized.targetObject != _config)
                Bind(_config);

            _serialized.Update();
            _halfWidth = _playfield.HalfWidth;

            EditorGUILayout.BeginHorizontal();
            DrawField();
            DrawPanel();
            EditorGUILayout.EndHorizontal();

            if (_serialized.ApplyModifiedProperties())
                Repaint();
        }

        private static PlayfieldConfig FindPlayfield()
        {
            string[] guids = AssetDatabase.FindAssets("t:PlayfieldConfig");

            if (guids.Length == 0)
                return null;

            return AssetDatabase.LoadAssetAtPath<PlayfieldConfig>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        private void Bind(LevelConfig config)
        {
            _config = config;
            _selected = -1;
            _dragIndex = -1;
            _serialized = null;

            if (_config == null)
                return;

            _serialized = new SerializedObject(_config);
            _levelLength = _serialized.FindProperty("levelLength");
            _scrollSpeed = _serialized.FindProperty("scrollSpeed");
            _upgradeCostScale = _serialized.FindProperty("upgradeCostScale");
            _entries = _serialized.FindProperty("entries");
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            LevelConfig picked = (LevelConfig)EditorGUILayout.ObjectField(_config, typeof(LevelConfig), false, GUILayout.Width(160f));

            if (picked != _config)
                Bind(picked);

            _playfield = (PlayfieldConfig)EditorGUILayout.ObjectField(_playfield, typeof(PlayfieldConfig), false, GUILayout.Width(140f));

            GUILayout.Space(12f);
            GUILayout.Label("Brush", EditorStyles.miniLabel, GUILayout.Width(38f));
            _brushType = (StackTypeConfig)EditorGUILayout.ObjectField(_brushType, typeof(StackTypeConfig), false, GUILayout.Width(130f));

            GUILayout.Label("HP", EditorStyles.miniLabel, GUILayout.Width(20f));
            _brushHp = EditorGUILayout.IntField(_brushHp, GUILayout.Width(40f));

            GUILayout.Space(12f);
            GUILayout.Label("Snap", EditorStyles.miniLabel, GUILayout.Width(34f));
            _snapIndex = EditorGUILayout.Popup(_snapIndex, SnapLabels, EditorStyles.toolbarPopup, GUILayout.Width(56f));

            GUILayout.Label("Zoom", EditorStyles.miniLabel, GUILayout.Width(38f));
            _zoom = GUILayout.HorizontalSlider(_zoom, 12f, 90f, GUILayout.Width(90f));

            if (_brushType == null)
                GUILayout.Label("Brush needs a stack type", EditorStyles.miniLabel);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawField()
        {
            float length = Mathf.Max(_config.LevelLength, 1f);
            float width = _halfWidth * 2f * _zoom;

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandWidth(true));

            Rect canvas = GUILayoutUtility.GetRect(
                width + GutterWidth,
                length * _zoom,
                GUILayout.ExpandWidth(false),
                GUILayout.ExpandHeight(false));

            _field = new Rect(canvas.x + GutterWidth, canvas.y, width, canvas.height);

            EditorGUI.DrawRect(_field, Backdrop);

            DrawDistanceLines(length);
            DrawWidthLines();
            DrawEntries();
            HandleInput(length);

            EditorGUILayout.EndScrollView();
        }

        private void DrawDistanceLines(float length)
        {
            int steps = Mathf.CeilToInt(length);
            int labelStep = _zoom < 22f ? MajorStep : 1;

            for (int i = 0; i <= steps; i++)
            {
                float y = DistanceToPixels(i);
                bool major = i % MajorStep == 0;

                EditorGUI.DrawRect(new Rect(_field.x, y, _field.width, 1f), major ? MajorLine : MinorLine);

                if (i % labelStep != 0)
                    continue;

                GUI.Label(new Rect(_field.x - GutterWidth, y - 8f, GutterWidth - 6f, 16f), i.ToString(), TickStyle());
            }
        }

        private void DrawWidthLines()
        {
            int steps = Mathf.FloorToInt(_halfWidth);

            for (int i = -steps; i <= steps; i++)
                EditorGUI.DrawRect(new Rect(WidthToPixels(i), _field.y, 1f, _field.height), i == 0 ? CentreLine : MinorLine);

            EditorGUI.DrawRect(new Rect(_field.x, _field.y, 1f, _field.height), EdgeLine);
            EditorGUI.DrawRect(new Rect(_field.xMax - 1f, _field.y, 1f, _field.height), EdgeLine);
        }

        private void DrawEntries()
        {
            for (int i = 0; i < _config.Entries.Count; i++)
                DrawEntry(i);
        }

        private void DrawEntry(int index)
        {
            StackGroupEntry entry = _config.Entries[index];
            Vector2 position = PositionOf(index);
            Rect box = ToPixels(Footprint(entry, position));

            EditorGUI.DrawRect(box, entry.motion == GroupMotion.Horizontal ? SweepFill : GroupFill);

            if (entry.stackType == null)
                EditorGUI.DrawRect(box, MissingType);
            else
                DrawMembers(entry, position);

            if (Conflicts(index))
                DrawOutline(box, ConflictOutline, 2f);
            else
                DrawOutline(box, GroupOutline, 1f);

            if (index == _selected)
                DrawOutline(new Rect(box.x + 2f, box.y + 2f, box.width - 4f, box.height - 4f), SelectedOutline, 2f);

            GUI.Label(box, Caption(entry), CaptionStyle());
        }

        // A capped entry earns a mark on the grid itself. The hp above the cap breaks
        // no plate, so it is damage the player deals for nothing.
        private static string Caption(StackGroupEntry entry)
        {
            if (entry.stackType == null)
                return "!";

            string plates = StackGroupGeometry.PlateCount(entry).ToString();

            return StackGroupGeometry.IsCapped(entry) ? plates + " !" : plates;
        }

        private void DrawMembers(StackGroupEntry entry, Vector2 position)
        {
            float half = entry.stackType.PlateSize.x * 0.5f;
            float height = StackGroupGeometry.StackHeight(entry);
            int count = StackGroupGeometry.MemberCount(entry);

            for (int i = 0; i < count; i++)
            {
                Vector2 offset = StackGroupGeometry.MemberOffset(entry, i, count);
                Rect member = new Rect(position.x + offset.x - half, position.y + offset.y, half * 2f, height);

                EditorGUI.DrawRect(ToPixels(member), entry.stackType.PlateColor);
            }
        }

        private static void DrawOutline(Rect box, Color color, float thickness)
        {
            EditorGUI.DrawRect(new Rect(box.x, box.y, box.width, thickness), color);
            EditorGUI.DrawRect(new Rect(box.x, box.yMax - thickness, box.width, thickness), color);
            EditorGUI.DrawRect(new Rect(box.x, box.y, thickness, box.height), color);
            EditorGUI.DrawRect(new Rect(box.xMax - thickness, box.y, thickness, box.height), color);
        }

        // The footprint is what the group actually covers on its way down, in world
        // units, so nothing here is rounded to a cell and nothing can go missing.
        private Rect Footprint(StackGroupEntry entry, Vector2 position)
        {
            if (entry.stackType == null)
                return new Rect(position.x - 0.5f, position.y, 1f, 1f);

            bool sweeps = entry.motion == GroupMotion.Horizontal;
            float halfWidth = sweeps ? _halfWidth : StackGroupGeometry.HalfWidth(entry);
            float centre = sweeps ? 0f : position.x;
            float bottom = position.y;
            float height = StackGroupGeometry.StackHeight(entry);

            // A ring hangs its members around the group origin, so the silhouette
            // reaches a radius further up and down than a plain stack does.
            if (entry.layout == GroupLayout.Ring || entry.layout == GroupLayout.Cluster)
            {
                bottom -= entry.radius;
                height += entry.radius * 2f;
            }

            return new Rect(centre - halfWidth, bottom, halfWidth * 2f, height);
        }

        private bool Conflicts(int index)
        {
            Rect box = Footprint(_config.Entries[index], PositionOf(index));

            for (int i = 0; i < _config.Entries.Count; i++)
            {
                if (i == index)
                    continue;

                if (box.Overlaps(Footprint(_config.Entries[i], PositionOf(i))))
                    return true;
            }

            return false;
        }

        private Vector2 PositionOf(int index)
        {
            if (index == _dragIndex)
                return _dragPosition;

            StackGroupEntry entry = _config.Entries[index];

            return new Vector2(entry.xPosition, entry.distance);
        }

        private float WidthToPixels(float worldX)
        {
            return _field.x + (worldX + _halfWidth) * _zoom;
        }

        private float DistanceToPixels(float distance)
        {
            return _field.yMax - distance * _zoom;
        }

        private Vector2 PixelsToWorld(Vector2 pixels)
        {
            return new Vector2(
                (pixels.x - _field.x) / _zoom - _halfWidth,
                (_field.yMax - pixels.y) / _zoom);
        }

        private Rect ToPixels(Rect world)
        {
            return new Rect(
                WidthToPixels(world.xMin),
                DistanceToPixels(world.yMax),
                world.width * _zoom,
                world.height * _zoom);
        }

        private void HandleInput(float length)
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            Event current = Event.current;

            switch (current.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    if (current.button != 0 || !_field.Contains(current.mousePosition))
                        return;

                    BeginDrag(PixelsToWorld(current.mousePosition), controlId, length);
                    current.Use();
                    Repaint();
                    return;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != controlId || _dragIndex < 0)
                        return;

                    _dragPosition = Place(
                        PixelsToWorld(current.mousePosition) - _dragGrab,
                        StackGroupGeometry.HalfWidth(_config.Entries[_dragIndex]),
                        length);

                    _dragMoved = _dragPosition != _dragAnchor;
                    current.Use();
                    Repaint();
                    return;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl != controlId)
                        return;

                    GUIUtility.hotControl = 0;

                    // A click that never moved writes nothing. Selecting an entry to
                    // read it must not round its position to the snap step.
                    if (_dragIndex >= 0 && _dragMoved)
                        MoveEntry(_dragIndex, _dragPosition);

                    _dragIndex = -1;
                    current.Use();
                    Repaint();
                    return;
            }
        }

        private void BeginDrag(Vector2 world, int controlId, float length)
        {
            int hit = EntryAt(world);

            if (hit < 0)
            {
                CreateEntry(Place(world, BrushHalfWidth(), length));
                return;
            }

            StackGroupEntry entry = _config.Entries[hit];

            _selected = hit;
            _dragIndex = hit;
            _dragAnchor = new Vector2(entry.xPosition, entry.distance);
            _dragPosition = _dragAnchor;
            _dragGrab = world - _dragAnchor;
            _dragMoved = false;

            GUIUtility.hotControl = controlId;
        }

        private int EntryAt(Vector2 world)
        {
            // Later entries draw on top, so they are the ones a click should find.
            for (int i = _config.Entries.Count - 1; i >= 0; i--)
            {
                if (Footprint(_config.Entries[i], PositionOf(i)).Contains(world))
                    return i;
            }

            return -1;
        }

        private float BrushHalfWidth()
        {
            return _brushType != null ? _brushType.PlateSize.x * 0.5f : 0.5f;
        }

        private Vector2 Place(Vector2 world, float entryHalfWidth, float length)
        {
            float step = SnapSteps[_snapIndex];
            float limit = Mathf.Max(_halfWidth - entryHalfWidth, 0f);

            return new Vector2(
                Mathf.Clamp(Snap(world.x, step), -limit, limit),
                Mathf.Clamp(Snap(world.y, step), 0f, length));
        }

        private static float Snap(float value, float step)
        {
            return step > 0f ? Mathf.Round(value / step) * step : value;
        }

        private void MoveEntry(int index, Vector2 position)
        {
            SerializedProperty entry = _entries.GetArrayElementAtIndex(index);

            entry.FindPropertyRelative("xPosition").floatValue = position.x;
            entry.FindPropertyRelative("distance").floatValue = position.y;

            _serialized.ApplyModifiedProperties();
        }

        private void CreateEntry(Vector2 position)
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

            entry.FindPropertyRelative("distance").floatValue = position.y;
            entry.FindPropertyRelative("xPosition").floatValue = position.x;
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
            EditorGUILayout.PropertyField(_upgradeCostScale);
            EditorGUILayout.LabelField("Entries", _entries.arraySize.ToString());
            EditorGUILayout.LabelField("Duration", Seconds(_config.LevelLength));

            EditorGUILayout.Space();

            if (_selected < 0 || _selected >= _entries.arraySize)
            {
                EditorGUILayout.HelpBox("Click empty space to place a stack, or a stack to select and drag it.", MessageType.None);
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
            else if (layoutValue == GroupLayout.Ring || layoutValue == GroupLayout.Cluster)
            {
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("count"));
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("radius"));
            }

            EditorGUILayout.PropertyField(motion);

            if ((GroupMotion)motion.enumValueIndex != GroupMotion.Static)
                EditorGUILayout.PropertyField(entry.FindPropertyRelative("motionSpeed"));

            StackGroupEntry selected = _config.Entries[_selected];

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Plates", StackGroupGeometry.PlateCount(selected).ToString());
            EditorGUILayout.LabelField("Max Hp", StackGroupGeometry.MaxHp(selected).ToString());
            EditorGUILayout.LabelField("Half Width", StackGroupGeometry.HalfWidth(selected).ToString("0.00"));
            EditorGUILayout.LabelField("Height", StackGroupGeometry.StackHeight(selected).ToString("0.00"));
            EditorGUILayout.LabelField("Spawns At", Seconds(selected.distance));

            if (StackGroupGeometry.IsCapped(selected))
                EditorGUILayout.HelpBox(CapWarning(selected), MessageType.Warning);

            EditorGUILayout.Space();

            if (GUILayout.Button("Delete Entry"))
            {
                _entries.DeleteArrayElementAtIndex(_selected);
                _serialized.ApplyModifiedProperties();
                _selected = -1;
                _dragIndex = -1;
            }

            EditorGUILayout.EndVertical();
        }

        private static string CapWarning(StackGroupEntry entry)
        {
            return "Hp " + entry.hp + " asks for " + StackGroupGeometry.RequestedPlateCount(entry)
                + " plates but this stack tops out at " + entry.stackType.MaxPlates
                + ". Damage past " + StackGroupGeometry.MaxHp(entry)
                + " breaks no plate, so it scores nothing and throws no shards.";
        }

        private string Seconds(float distance)
        {
            if (_config.ScrollSpeed <= 0f)
                return "-";

            return (distance / _config.ScrollSpeed).ToString("0.0") + " s";
        }

        private GUIStyle CaptionStyle()
        {
            if (_captionStyle == null)
            {
                _captionStyle = new GUIStyle(EditorStyles.boldLabel);
                _captionStyle.alignment = TextAnchor.MiddleCenter;
                _captionStyle.normal.textColor = Color.white;
            }

            return _captionStyle;
        }

        private GUIStyle TickStyle()
        {
            if (_tickStyle == null)
            {
                _tickStyle = new GUIStyle(EditorStyles.miniLabel);
                _tickStyle.alignment = TextAnchor.MiddleRight;
            }

            return _tickStyle;
        }
    }
}
