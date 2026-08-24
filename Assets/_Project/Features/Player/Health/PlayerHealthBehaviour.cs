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
        private GameStateMachine _state;

        [Inject]
        public void Construct(PlayerHealth health, GameStateMachine state)
        {
            _health = health;
            _state = state;
        }

        private void Start()
        {
            _state.Changed += OnStateChanged;
        }

        private void OnDestroy()
        {
            _state.Changed -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Playing)
                _health.Restore();
        }

        private void Update()
        {
            if (_state.Current != GameState.Playing)
            {
                shield.SetActive(false);
                return;
            }

            _health.Tick(Time.deltaTime);
            shield.SetActive(_health.IsInvulnerable);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_state.Current != GameState.Playing)
                return;

            if (!other.TryGetComponent(out IHazard hazard))
                return;

            if (!_health.TryTakeHit(hazard.ContactDamage))
                return;

            if (_health.IsDead)
                _state.Set(GameState.Lost);
        }
    }
}
