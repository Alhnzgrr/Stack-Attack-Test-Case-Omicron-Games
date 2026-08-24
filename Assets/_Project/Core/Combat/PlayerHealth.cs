namespace StackAttack.Core
{
    public class PlayerHealth
    {
        private readonly float _invulnerabilityDuration;

        private float _invulnerabilityLeft;

        public PlayerHealth(int maxHealth, float invulnerabilityDuration)
        {
            Max = maxHealth;
            _invulnerabilityDuration = invulnerabilityDuration;
            Current = maxHealth;
        }

        public int Max { get; }

        public int Current { get; private set; }

        public bool IsInvulnerable => _invulnerabilityLeft > 0f;

        public bool IsDead => Current <= 0;

        public void Restore()
        {
            Current = Max;
            _invulnerabilityLeft = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (_invulnerabilityLeft > 0f)
                _invulnerabilityLeft -= deltaTime;
        }

        // Reports whether the hit actually landed, so the caller only reacts to real damage.
        public bool TryTakeHit(int amount)
        {
            if (IsInvulnerable || IsDead)
                return false;

            Current -= amount;
            _invulnerabilityLeft = _invulnerabilityDuration;
            return true;
        }
    }
}
