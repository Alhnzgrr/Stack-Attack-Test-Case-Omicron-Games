using StackAttack.Core;
using UnityEditor;
using UnityEngine;

namespace StackAttack.Upgrades
{
    public static class UpgradeCardBuilder
    {
        private const string PoolAsset = "Assets/_Project/ScriptableObjects/Upgrade/UpgradePool.asset";

        [MenuItem("StackAttack/Add Skill Cards")]
        private static void Build()
        {
            UpgradePool pool = AssetDatabase.LoadAssetAtPath<UpgradePool>(PoolAsset);
            SerializedObject serialized = new SerializedObject(pool);
            SerializedProperty options = serialized.FindProperty("options");

            AddCard(options, UpgradeKind.BoomerangCount, "BOOMERANG", 1f, new Color(1f, 0.82f, 0.24f, 1f));
            AddCard(options, UpgradeKind.RocketDamage, "ROCKET", 4f, new Color(1f, 0.45f, 0.2f, 1f));

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(pool);
            AssetDatabase.SaveAssets();

            Debug.Log("Skill cards are in the pool.");
        }

        // Rerunning the menu item must not stack duplicates into the pool, and a kind
        // already in there is one the designer may have tuned since.
        private static void AddCard(SerializedProperty options, UpgradeKind kind, string statName, float amount, Color tint)
        {
            for (int i = 0; i < options.arraySize; i++)
            {
                if (options.GetArrayElementAtIndex(i).FindPropertyRelative("kind").enumValueIndex == (int)kind)
                    return;
            }

            int index = options.arraySize;
            options.InsertArrayElementAtIndex(index);

            SerializedProperty option = options.GetArrayElementAtIndex(index);

            option.FindPropertyRelative("kind").enumValueIndex = (int)kind;
            option.FindPropertyRelative("category").stringValue = "SKILL";
            option.FindPropertyRelative("statName").stringValue = statName;
            option.FindPropertyRelative("amount").floatValue = amount;
            option.FindPropertyRelative("icon").objectReferenceValue = null;
            option.FindPropertyRelative("tint").colorValue = tint;
        }
    }
}
