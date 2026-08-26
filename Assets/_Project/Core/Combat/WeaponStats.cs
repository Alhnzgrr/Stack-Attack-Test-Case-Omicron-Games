namespace StackAttack.Core
{
    public class WeaponStats
    {
        private readonly float _baseDamage;
        private readonly float _baseFireRate;
        private readonly int _baseProjectileCount;
        private readonly float _baseProjectileSize;

        public WeaponStats(
            float damage,
            float fireRate,
            int projectileCount,
            float projectileSize,
            float boomerangInterval,
            float rocketInterval)
        {
            _baseDamage = damage;
            _baseFireRate = fireRate;
            _baseProjectileCount = projectileCount;
            _baseProjectileSize = projectileSize;

            BoomerangInterval = boomerangInterval;
            RocketInterval = rocketInterval;

            ResetToBase();
        }

        public float Damage;
        public float FireRate;
        public int ProjectileCount;
        public float ProjectileSize;
        public int Pierce;

        // Both skills start switched off. A count of zero and no rocket damage mean
        // nothing is thrown until a card turns them on.
        public int BoomerangCount;
        public float RocketDamage;

        // How often a skill comes round is tuning rather than reward, so it is set
        // once and left alone by both the upgrades and the reset.
        public readonly float BoomerangInterval;
        public readonly float RocketInterval;

        // Upgrades are run-scoped, so the numbers a level starts from have to survive
        // being overwritten during that level.
        public void ResetToBase()
        {
            Damage = _baseDamage;
            FireRate = _baseFireRate;
            ProjectileCount = _baseProjectileCount;
            ProjectileSize = _baseProjectileSize;
            Pierce = 0;
            BoomerangCount = 0;
            RocketDamage = 0f;
        }
    }
}
