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

        public void Restore()
        {
            Current = Max;
            _invulnerabilityLeft = 0f;
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
