using System;
using UnityEngine;

namespace StackAttack.Core
{
    public class LevelProgress
    {
        private int _levelIndex = -1;

        public float Length;
        public float Travelled;

        // Travelled moves every frame and is meant to be read that way. The index
        // changes a handful of times a run, so it announces itself rather than
        // leaving readers to notice on their own.
        public event Action LevelChanged;

        public int LevelIndex
        {
            get => _levelIndex;
            set
            {
                if (_levelIndex == value)
                    return;

                _levelIndex = value;
                LevelChanged?.Invoke();
            }
        }

        public float Normalized => Length > 0f ? Mathf.Clamp01(Travelled / Length) : 0f;

        public bool IsCompleted => Travelled >= Length;
    }
}
