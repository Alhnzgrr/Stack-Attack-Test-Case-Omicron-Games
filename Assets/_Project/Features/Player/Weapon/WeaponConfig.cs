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
        [SerializeField] private float boomerangInterval = 4f;
        [SerializeField] private float rocketInterval = 5f;

        public WeaponStats CreateStats()
        {
            return new WeaponStats(damage, fireRate, projectileCount, projectileSize, boomerangInterval, rocketInterval);
        }
    }
}
