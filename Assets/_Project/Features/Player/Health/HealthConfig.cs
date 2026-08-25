using UnityEngine;

namespace StackAttack.Player
{
    [CreateAssetMenu(fileName = "HealthConfig", menuName = "StackAttack/Player/Health Config")]
    public class HealthConfig : ScriptableObject
    {
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float invulnerabilityDuration = 2f;

        [SerializeField] private Color hitFlashColor = new Color(0.95f, 0.15f, 0.2f, 1f);
        [SerializeField] private float hitFlashDuration = 0.18f;
        [SerializeField] private float hitFlashStrength = 1f;
        [SerializeField] private float hitPunchScale = 0.18f;

        public int MaxHealth => maxHealth;
        public float InvulnerabilityDuration => invulnerabilityDuration;
        public Color HitFlashColor => hitFlashColor;
        public float HitFlashDuration => hitFlashDuration;
        public float HitFlashStrength => hitFlashStrength;
        public float HitPunchScale => hitPunchScale;
    }
}
