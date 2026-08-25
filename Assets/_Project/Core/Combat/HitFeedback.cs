using UnityEngine;

namespace StackAttack.Core
{
    public class HitFeedback
    {
        private readonly float _duration;
        private readonly float _flashStrength;
        private readonly float _punchScale;

        private float _elapsed;

        public HitFeedback(float duration, float flashStrength, float punchScale)
        {
            _duration = duration;
            _flashStrength = flashStrength;
            _punchScale = punchScale;
            _elapsed = duration;
        }

        public bool IsActive => _elapsed < _duration;

        // A new hit restarts the pulse instead of adding to it, so rapid hits read as
        // a flicker rather than a colour that saturates and never comes back down.
        public void Restart()
        {
            _elapsed = 0f;
        }

        public void Reset()
        {
            _elapsed = _duration;
        }

        public void Tick(float deltaTime)
        {
            _elapsed += deltaTime;
        }

        public float FlashAmount => _flashStrength * Mathf.SmoothStep(1f, 0f, Progress);

        // Sin over half a period leaves and returns to exactly 1, so the target can
        // never be left stuck at a punched scale.
        public float ScaleMultiplier => 1f + _punchScale * Mathf.Sin(Progress * Mathf.PI);

        private float Progress => _duration <= 0f ? 1f : Mathf.Clamp01(_elapsed / _duration);
    }
}
