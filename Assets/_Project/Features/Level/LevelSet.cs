using System;
using UnityEngine;

namespace StackAttack.Level
{
    [CreateAssetMenu(fileName = "LevelSet", menuName = "StackAttack/Level/Level Set")]
    public class LevelSet : ScriptableObject
    {
        [SerializeField] private LevelConfig[] levels = Array.Empty<LevelConfig>();

        public int Count => levels.Length;

        // A saved index can point past the set when levels are removed, so it is clamped
        // rather than trusted.
        public LevelConfig Get(int index)
        {
            return levels[Mathf.Clamp(index, 0, levels.Length - 1)];
        }
    }
}
