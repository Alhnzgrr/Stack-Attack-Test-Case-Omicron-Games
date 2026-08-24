using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerHealthBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject shield;

        private PlayerHealth _health;

        [Inject]
        public void Construct(HealthConfig config)
        {
            _health = new PlayerHealth(config.MaxHealth, config.InvulnerabilityDuration);
        }

        private void Update()
        {
            _health.Tick(Time.deltaTime);
            shield.SetActive(_health.IsInvulnerable);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.TryGetComponent(out IHazard hazard))
                return;

            if (!_health.TryTakeHit(hazard.ContactDamage))
                return;

            if (_health.IsDead)
                Die();
        }

        private void Die()
        {
            Time.timeScale = 0f;
        }
    }
}
