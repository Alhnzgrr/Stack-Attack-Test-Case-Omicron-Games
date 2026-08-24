using UnityEngine;

namespace StackAttack.Player
{
    [CreateAssetMenu(fileName = "HealthConfig", menuName = "StackAttack/Player/Health Config")]
    public class HealthConfig : ScriptableObject
    {
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float invulnerabilityDuration = 2f;

        public int MaxHealth => maxHealth;
        public float InvulnerabilityDuration => invulnerabilityDuration;
    }
}
