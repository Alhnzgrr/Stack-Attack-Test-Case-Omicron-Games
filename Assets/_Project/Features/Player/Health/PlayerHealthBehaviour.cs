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
        [SerializeField] private SpriteRenderer body;

        private PlayerHealth _health;
        private GameStateMachine _state;
        private HealthConfig _config;

        private HitFeedback _feedback;
        private Color _baseColor;
        private Vector3 _baseScale;

        [Inject]
        public void Construct(PlayerHealth health, GameStateMachine state, HealthConfig config)
        {
            _health = health;
            _state = state;
            _config = config;
            _feedback = new HitFeedback(config.HitFlashDuration, config.HitFlashStrength, config.HitPunchScale);
        }

        private void Awake()
        {
            _baseColor = body.color;
            _baseScale = body.transform.localScale;
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
            if (state != GameState.Playing)
                return;

            _health.Restore();
            _feedback.Reset();
            ApplyFeedback();
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

            if (!_feedback.IsActive)
                return;

            _feedback.Tick(Time.deltaTime);
            ApplyFeedback();
        }

        // The flash is deliberately much shorter than the invulnerability window: the
        // shield ring says "you are safe for a while", the punch says "you were hit".
        private void ApplyFeedback()
        {
            body.color = Color.Lerp(_baseColor, _config.HitFlashColor, _feedback.FlashAmount);
            body.transform.localScale = _baseScale * _feedback.ScaleMultiplier;
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
            {
                _state.Set(GameState.Lost);
                return;
            }

            _feedback.Restart();
            ApplyFeedback();
        }
    }
}
