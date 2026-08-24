using StackAttack.Core;

namespace StackAttack.Player
{
    public class AutoWeapon
    {
        private readonly WeaponStats _stats;

        private float _cooldown;

        public AutoWeapon(WeaponStats stats)
        {
            _stats = stats;
        }

        // The cooldown keeps draining while the finger is up, so releasing and tapping
        // again can never produce a faster rate than the stats allow.
        public bool Tick(float deltaTime, bool isPressed)
        {
            if (deltaTime <= 0f)
                return false;

            if (_cooldown > 0f)
                _cooldown -= deltaTime;

            if (!isPressed || _cooldown > 0f)
                return false;

            _cooldown = 1f / _stats.FireRate;
            return true;
        }
    }
}
