using System.Collections.Generic;
using StackAttack.Core;
using UnityEditor;
using UnityEngine;

namespace StackAttack.Level
{
    public static class LevelBuilder
    {
        private const string StackFolder = "Assets/_Project/ScriptableObjects/Stack/";
        private const string LevelFolder = "Assets/_Project/ScriptableObjects/Levels/Levels/";
        private const float ScrollSpeed = 0.4f;

        private class Plan
        {
            public float distance;
            public float x;
            public StackTypeConfig type;
            public int hp;
            public GroupLayout layout = GroupLayout.Single;
            public int count = 1;
            public float spacing = 1f;
            public float radius = 1f;
            public GroupMotion motion = GroupMotion.Static;
            public float motionSpeed;
        }

        [MenuItem("StackAttack/Build Levels 2-4")]
        private static void Build()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Build Levels 2-4",
                "This replaces every entry in Level_02, Level_03 and Level_04. Level_01 is left alone.",
                "Replace",
                "Cancel");

            if (!confirmed)
                return;

            StackTypeConfig swarm = LoadType("Swarm");
            StackTypeConfig normal = LoadType("Normal");
            StackTypeConfig armored = LoadType("Armored");

            Write("Level_02", 25f, 1.0f, BuildTwo(swarm, normal, armored));
            Write("Level_03", 28f, 1.3f, BuildThree(swarm, normal, armored));
            Write("Level_04", 29f, 1.3f, BuildFour(swarm, normal, armored));

            AssetDatabase.SaveAssets();
            Debug.Log("Levels 2-4 rebuilt.");
        }

        // Level two is about rows on the move: a tight zigzag of single colours to
        // open, pairs sweeping through the middle, then four rows of three closing it.
        private static List<Plan> BuildTwo(StackTypeConfig swarm, StackTypeConfig normal, StackTypeConfig armored)
        {
            return new List<Plan>
            {
                MakeSingle(0.5f, 0f, swarm, 8),
                MakeSingle(1.0f, -1f, normal, 8),
                MakeSingle(1.5f, 1f, armored, 12),
                MakeSingle(2.0f, -2f, swarm, 10),
                MakeSingle(2.5f, 2f, normal, 12),
                MakeSingle(3.0f, 0f, armored, 16),
                MakeSingle(3.5f, -1f, swarm, 12),
                MakeSingle(4.0f, 1f, normal, 14),

                MakeRow(6.0f, 0f, normal, 16, 2, 1.2f, 0.8f),
                MakeRow(8.0f, -1f, swarm, 12, 2, 1.2f, 1.0f),
                MakeRow(10.5f, 1f, armored, 16, 2, 1.2f, 1.2f),
                MakeSingle(12.0f, 0f, swarm, 6),

                MakeRow(14.5f, 0f, normal, 12, 3, 1.0f, 1.0f),
                MakeRow(16.5f, 0f, normal, 16, 3, 1.0f, 1.2f),
                MakeRow(19.0f, 0f, armored, 16, 3, 1.1f, 1.4f),
                MakeRow(21.0f, 0f, armored, 24, 3, 1.1f, 1.6f)
            };
        }

        // Level three introduces the ring. Singles first, then rings standing still
        // with singles framing them at the edges, then rings that sweep.
        private static List<Plan> BuildThree(StackTypeConfig swarm, StackTypeConfig normal, StackTypeConfig armored)
        {
            return new List<Plan>
            {
                MakeSingle(0.5f, 0f, swarm, 10),
                MakeSingle(1.0f, -2f, normal, 8),
                MakeSingle(1.0f, 2f, normal, 8),
                MakeSingle(2.5f, -1f, swarm, 12),
                MakeSingle(2.5f, 1f, swarm, 12),
                MakeSingle(4.0f, 0f, armored, 20),

                MakeRing(6.5f, normal, 12, 3, 0.7f, GroupMotion.Static, 0f),
                MakeSingle(7.5f, -2f, swarm, 12),
                MakeSingle(7.5f, 2f, swarm, 12),
                MakeRing(10.5f, armored, 16, 4, 0.75f, GroupMotion.Static, 0f),
                MakeSingle(12.0f, -2f, normal, 16),
                MakeSingle(12.0f, 2f, normal, 16),
                MakeRing(14.5f, armored, 16, 5, 0.8f, GroupMotion.Static, 0f),

                MakeRing(17.5f, normal, 12, 4, 0.75f, GroupMotion.Horizontal, 1.0f),
                MakeRing(21.0f, armored, 16, 5, 0.8f, GroupMotion.Horizontal, 1.2f),
                MakeRing(24.0f, armored, 16, 6, 0.87f, GroupMotion.Horizontal, 1.4f)
            };
        }

        // Level four spins them. The ring turns instead of sliding, and the last two
        // are clusters, where the core holds still while the ring wheels around it.
        private static List<Plan> BuildFour(StackTypeConfig swarm, StackTypeConfig normal, StackTypeConfig armored)
        {
            return new List<Plan>
            {
                MakeSingle(0.5f, 0f, swarm, 10),
                MakeSingle(1.0f, -2f, normal, 10),
                MakeSingle(1.0f, 2f, normal, 10),
                MakeSingle(2.5f, -1f, armored, 16),
                MakeSingle(2.5f, 1f, armored, 16),

                MakeRing(5.0f, normal, 12, 3, 0.7f, GroupMotion.Orbit, 60f),
                MakeSingle(6.5f, -2f, swarm, 12),
                MakeSingle(6.5f, 2f, swarm, 12),
                MakeRing(9.0f, armored, 16, 4, 0.75f, GroupMotion.Orbit, 80f),
                MakeSingle(11.0f, -2f, normal, 16),
                MakeSingle(11.0f, 2f, normal, 16),
                MakeRing(13.5f, armored, 12, 5, 0.8f, GroupMotion.Orbit, 100f),

                MakeRing(17.0f, armored, 16, 6, 0.87f, GroupMotion.Orbit, 110f),
                MakeCluster(20.5f, armored, 16, 7, 0.87f, 120f),
                MakeCluster(24.0f, armored, 16, 8, 0.87f, 130f)
            };
        }

        private static Plan MakeSingle(float distance, float x, StackTypeConfig type, int hp)
        {
            return new Plan { distance = distance, x = x, type = type, hp = hp };
        }

        private static Plan MakeRow(float distance, float x, StackTypeConfig type, int hp, int count, float spacing, float sweep)
        {
            return new Plan
            {
                distance = distance,
                x = x,
                type = type,
                hp = hp,
                layout = GroupLayout.Row,
                count = count,
                spacing = spacing,
                motion = GroupMotion.Horizontal,
                motionSpeed = sweep
            };
        }

        // Rings and clusters sit on the centre line: their reach leaves so little room
        // either side that an offset would only be clamped away at spawn.
        private static Plan MakeRing(float distance, StackTypeConfig type, int hp, int count, float radius, GroupMotion motion, float speed)
        {
            return new Plan
            {
                distance = distance,
                x = 0f,
                type = type,
                hp = hp,
                layout = GroupLayout.Ring,
                count = count,
                radius = radius,
                motion = motion,
                motionSpeed = speed
            };
        }

        private static Plan MakeCluster(float distance, StackTypeConfig type, int hp, int count, float radius, float spin)
        {
            return new Plan
            {
                distance = distance,
                x = 0f,
                type = type,
                hp = hp,
                layout = GroupLayout.Cluster,
                count = count,
                radius = radius,
                motion = GroupMotion.Orbit,
                motionSpeed = spin
            };
        }

        private static StackTypeConfig LoadType(string name)
        {
            return AssetDatabase.LoadAssetAtPath<StackTypeConfig>(StackFolder + name + ".asset");
        }

        private static void Write(string name, float length, float upgradeCostScale, List<Plan> plans)
        {
            LevelConfig config = AssetDatabase.LoadAssetAtPath<LevelConfig>(LevelFolder + name + ".asset");
            SerializedObject serialized = new SerializedObject(config);

            serialized.FindProperty("levelLength").floatValue = length;
            serialized.FindProperty("scrollSpeed").floatValue = ScrollSpeed;
            serialized.FindProperty("upgradeCostScale").floatValue = upgradeCostScale;

            SerializedProperty entries = serialized.FindProperty("entries");
            entries.ClearArray();

            for (int i = 0; i < plans.Count; i++)
            {
                entries.InsertArrayElementAtIndex(i);

                Plan plan = plans[i];
                SerializedProperty entry = entries.GetArrayElementAtIndex(i);

                entry.FindPropertyRelative("distance").floatValue = plan.distance;
                entry.FindPropertyRelative("xPosition").floatValue = plan.x;
                entry.FindPropertyRelative("stackType").objectReferenceValue = plan.type;
                entry.FindPropertyRelative("hp").intValue = plan.hp;
                entry.FindPropertyRelative("layout").enumValueIndex = (int)plan.layout;
                entry.FindPropertyRelative("count").intValue = plan.count;
                entry.FindPropertyRelative("spacing").floatValue = plan.spacing;
                entry.FindPropertyRelative("radius").floatValue = plan.radius;
                entry.FindPropertyRelative("motion").enumValueIndex = (int)plan.motion;
                entry.FindPropertyRelative("motionSpeed").floatValue = plan.motionSpeed;
            }

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(config);
        }
    }
}
