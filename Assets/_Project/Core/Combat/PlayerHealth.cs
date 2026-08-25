using System;

namespace StackAttack.Core
{
    public class PlayerHealth
    {
        private readonly int _baseMax;
        private readonly float _invulnerabilityDuration;

        private float _invulnerabilityLeft;

        public PlayerHealth(int maxHealth, float invulnerabilityDuration)
        {
            _baseMax = maxHealth;
            _invulnerabilityDuration = invulnerabilityDuration;

            ResetToBase();
        }

        public int Max { get; private set; }

        public int Current { get; private set; }

        public bool IsInvulnerable => _invulnerabilityLeft > 0f;

        public bool IsDead => Current <= 0;

        // Raised on every mutation. One signal rather than one per cause: every
        // listener redraws the same way whether a heart was lost, granted or reset.
        public event Action Changed;

        public void Restore()
        {
            Current = Max;
            _invulnerabilityLeft = 0f;
            Changed?.Invoke();
        }

        // Extra hearts are an upgrade, and upgrades do not survive the level that
        // granted them.
        public void ResetToBase()
        {
            Max = _baseMax;
            Restore();
        }

        public void Grow(int amount)
        {
            Max += amount;
            Current += amount;
            Changed?.Invoke();
        }

        public void Tick(float deltaTime)
        {
            if (_invulnerabilityLeft <= 0f)
                return;

            _invulnerabilityLeft -= deltaTime;

            // Invulnerability lapsing is the one change nobody calls a method for:
            // it just runs out. Announcing it here is what lets the shield ring stop
            // being polled.
            if (_invulnerabilityLeft <= 0f)
                Changed?.Invoke();
        }

        // Reports whether the hit actually landed, so the caller only reacts to real damage.
        public bool TryTakeHit(int amount)
        {
            if (IsInvulnerable || IsDead)
                return false;

            Current -= amount;
            _invulnerabilityLeft = _invulnerabilityDuration;
            Changed?.Invoke();
            return true;
        }
    }
}
