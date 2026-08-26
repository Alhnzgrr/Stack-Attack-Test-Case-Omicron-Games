using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    public class PlayerWeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private Transform body;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private int poolCapacity = 24;
        [SerializeField] private float firePunchDuration = 0.1f;
        [SerializeField] private float firePunchScale = 0.12f;

        private IPointerInput _input;
        private GameStateMachine _state;
        private WeaponStats _stats;
        private ProjectileField _field;
        private AutoWeapon _weapon;
        private ThrownPool<Projectile> _pool;
        private IAudioService _audio;
        private HitFeedback _punch;
        private Vector3 _bodyScale;

        private void Awake()
        {
            _bodyScale = body.localScale;
            _punch = new HitFeedback(firePunchDuration, 0f, firePunchScale);
        }

        [Inject]
        public void Construct(IPointerInput input, WeaponStats stats, ProjectileField field, IAudioService audio, GameStateMachine state)
        {
            _input = input;
            _state = state;
            _stats = stats;
            _field = field;
            _audio = audio;
            _weapon = new AutoWeapon(stats);
            _pool = new ThrownPool<Projectile>(projectilePrefab, field.Root, poolCapacity);
        }

        private void Start()
        {
            _field.Cleared += _pool.Clear;
        }

        private void OnDestroy()
        {
            _field.Cleared -= _pool.Clear;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            if (_state.Current == GameState.Playing && _weapon.Tick(deltaTime, _input.IsPressed))
                Fire();

            // Outside the state check so a shot fired on the last frame of a run still
            // settles back, rather than leaving the body stuck mid punch.
            Punch(deltaTime);
        }

        private void Punch(float deltaTime)
        {
            if (!_punch.IsActive)
                return;

            _punch.Tick(deltaTime);
            body.localScale = _bodyScale * _punch.ScaleMultiplier;
        }

        private void Fire()
        {
            // Restarting rather than stacking: a faster fire rate reads as a flutter
            // instead of a body that swells and never comes back down.
            _punch.Restart();

            // One shot per volley, not one per projectile: five bullets leaving
            // together are a single trigger pull to the ear.
            _audio.Play(GameSound.Shot);

            float size = _stats.ProjectileSize;
            float spread = (_stats.ProjectileCount - 1) * 0.5f * size;

            for (int i = 0; i < _stats.ProjectileCount; i++)
            {
                Vector3 position = muzzle.position + Vector3.right * (i * size - spread);
                Projectile projectile = _pool.Take(position);

                projectile.Launch(_stats.Damage, size, _stats.Pierce, _pool);
            }
        }
    }
}
