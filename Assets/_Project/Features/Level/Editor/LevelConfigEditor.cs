using StackAttack.Core;
using UnityEditor;
using UnityEngine;

namespace StackAttack.Level
{
    [CustomEditor(typeof(LevelConfig))]
    public class LevelConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty _levelLength;
        private SerializedProperty _scrollSpeed;
        private SerializedProperty _upgradeCostScale;
        private SerializedProperty _entries;

        private void OnEnable()
        {
            _levelLength = serializedObject.FindProperty("levelLength");
            _scrollSpeed = serializedObject.FindProperty("scrollSpeed");
            _upgradeCostScale = serializedObject.FindProperty("upgradeCostScale");
            _entries = serializedObject.FindProperty("entries");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_levelLength);
            EditorGUILayout.PropertyField(_scrollSpeed);
            EditorGUILayout.PropertyField(_upgradeCostScale);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Entries: " + _entries.arraySize, EditorStyles.boldLabel);

            int removeAt = -1;

            for (int i = 0; i < _entries.arraySize; i++)
            {
                if (DrawEntry(i))
                    removeAt = i;
            }

            if (removeAt >= 0)
                _entries.DeleteArrayElementAtIndex(removeAt);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Entry"))
                _entries.InsertArrayElementAtIndex(_entries.arraySize);

            if (GUILayout.Button("Sort By Distance"))
                SortByDistance();

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private bool DrawEntry(int index)
        {
            SerializedProperty entry = _entries.GetArrayElementAtIndex(index);

            SerializedProperty distance = entry.FindPropertyRelative("distance");
            SerializedProperty xPosition = entry.FindPropertyRelative("xPosition");
            SerializedProperty stackType = entry.FindPropertyRelative("stackType");
            SerializedProperty hp = entry.FindPropertyRelative("hp");
            SerializedProperty layout = entry.FindPropertyRelative("layout");
            SerializedProperty count = entry.FindPropertyRelative("count");
            SerializedProperty spacing = entry.FindPropertyRelative("spacing");
            SerializedProperty radius = entry.FindPropertyRelative("radius");
            SerializedProperty motion = entry.FindPropertyRelative("motion");
            SerializedProperty motionSpeed = entry.FindPropertyRelative("motionSpeed");
            SerializedProperty boss = entry.FindPropertyRelative("boss");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            string title = boss.objectReferenceValue != null
                ? "BOSS"
                : ((GroupLayout)layout.enumValueIndex).ToString();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(index + " - " + title, EditorStyles.boldLabel);
            bool remove = GUILayout.Button("X", GUILayout.Width(24f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(distance);
            EditorGUILayout.PropertyField(xPosition);
            EditorGUILayout.PropertyField(boss);

            if (boss.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(stackType, new GUIContent("Guard Type"));
                EditorGUILayout.PropertyField(hp, new GUIContent("Guard Hp"));
                EditorGUILayout.PropertyField(count, new GUIContent("Guard Count"));
                EditorGUILayout.PropertyField(radius, new GUIContent("Guard Radius"));
                EditorGUILayout.PropertyField(motionSpeed, new GUIContent("Guard Spin"));

                DrawPlateReadout(stackType, hp);

                EditorGUILayout.EndVertical();

                return remove;
            }

            EditorGUILayout.PropertyField(stackType);
            EditorGUILayout.PropertyField(hp);
            EditorGUILayout.PropertyField(layout);

            GroupLayout layoutValue = (GroupLayout)layout.enumValueIndex;

            if (layoutValue == GroupLayout.Row)
            {
                EditorGUILayout.PropertyField(count);
                EditorGUILayout.PropertyField(spacing);
            }
            else if (layoutValue == GroupLayout.Ring || layoutValue == GroupLayout.Cluster)
            {
                EditorGUILayout.PropertyField(count);
                EditorGUILayout.PropertyField(radius);
            }

            EditorGUILayout.PropertyField(motion);

            if ((GroupMotion)motion.enumValueIndex != GroupMotion.Static)
                EditorGUILayout.PropertyField(motionSpeed);

            DrawPlateReadout(stackType, hp);

            EditorGUILayout.EndVertical();

            return remove;
        }

        private static void DrawPlateReadout(SerializedProperty stackType, SerializedProperty hp)
        {
            StackTypeConfig type = stackType.objectReferenceValue as StackTypeConfig;

            if (type == null || type.HitsPerPlate <= 0)
                return;

            int requested = Mathf.CeilToInt(hp.intValue / (float)type.HitsPerPlate);
            int plates = Mathf.Min(requested, type.MaxPlates);

            EditorGUILayout.LabelField("Plates", plates.ToString());

            if (requested <= type.MaxPlates)
                return;

            EditorGUILayout.HelpBox(
                "Hp " + hp.intValue + " asks for " + requested + " plates but this stack tops out at "
                + type.MaxPlates + ". Damage past " + type.MaxPlates * type.HitsPerPlate
                + " breaks no plate, so it scores nothing and throws no shards.",
                MessageType.Warning);
        }

        private void SortByDistance()
        {
            for (int i = 1; i < _entries.arraySize; i++)
            {
                for (int j = i; j > 0; j--)
                {
                    float current = _entries.GetArrayElementAtIndex(j).FindPropertyRelative("distance").floatValue;
                    float previous = _entries.GetArrayElementAtIndex(j - 1).FindPropertyRelative("distance").floatValue;

                    if (previous <= current)
                        break;

                    _entries.MoveArrayElement(j, j - 1);
                }
            }
        }
    }
}
