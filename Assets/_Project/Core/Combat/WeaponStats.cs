namespace StackAttack.Core
{
    public class WeaponStats
    {
        private readonly float _baseDamage;
        private readonly float _baseFireRate;
        private readonly int _baseProjectileCount;
        private readonly float _baseProjectileSize;

        public WeaponStats(float damage, float fireRate, int projectileCount, float projectileSize)
        {
            _baseDamage = damage;
            _baseFireRate = fireRate;
            _baseProjectileCount = projectileCount;
            _baseProjectileSize = projectileSize;

            ResetToBase();
        }

        public float Damage;
        public float FireRate;
        public int ProjectileCount;
        public float ProjectileSize;
        public int Pierce;

        // Upgrades are run-scoped, so the numbers a level starts from have to survive
        // being overwritten during that level.
        public void ResetToBase()
        {
            Damage = _baseDamage;
            FireRate = _baseFireRate;
            ProjectileCount = _baseProjectileCount;
            ProjectileSize = _baseProjectileSize;
            Pierce = 0;
        }
    }
}
