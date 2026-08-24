using StackAttack.Core;
using UnityEngine;

namespace StackAttack.Player
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "StackAttack/Player/Weapon Config")]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private float damage = 1f;
        [SerializeField] private float fireRate = 6f;
        [SerializeField] private int projectileCount = 1;
        [SerializeField] private float projectileSize = 0.45f;

        public WeaponStats CreateStats()
        {
            return new WeaponStats
            {
                Damage = damage,
                FireRate = fireRate,
                ProjectileCount = projectileCount,
                ProjectileSize = projectileSize
            };
        }
    }
}
