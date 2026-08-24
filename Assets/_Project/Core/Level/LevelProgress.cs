using UnityEngine;

namespace StackAttack.Core
{
    public class LevelProgress
    {
        public int LevelIndex;
        public float Length;
        public float Travelled;

        public float Normalized => Length > 0f ? Mathf.Clamp01(Travelled / Length) : 0f;

        public bool IsCompleted => Travelled >= Length;
    }
}
