using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    public class PlayerRocketBehaviour : MonoBehaviour
    {
        [SerializeField] private Rocket rocketPrefab;
        [SerializeField] private int poolCapacity = 8;

        private GameStateMachine _state;
        private WeaponStats _stats;
        private ProjectileField _field;
        private IShardBurst _shards;
        private IAudioService _audio;
        private SkillTimer _timer;
        private ThrownPool<Rocket> _pool;

        [Inject]
        public void Construct(WeaponStats stats, ProjectileField field, IShardBurst shards, IAudioService audio, GameStateMachine state)
        {
            _stats = stats;
            _field = field;
            _shards = shards;
            _audio = audio;
            _state = state;
            _timer = new SkillTimer(stats.RocketInterval);
            _pool = new ThrownPool<Rocket>(rocketPrefab, field.Root, poolCapacity);
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
            if (_state.Current != GameState.Playing)
                return;

            if (!_timer.Tick(Time.deltaTime))
                return;

            // No damage means no card taken, and a rocket that hits for nothing is
            // worse than no rocket at all.
            if (_stats.RocketDamage <= 0f)
                return;

            Rocket rocket = _pool.Take(transform.position);

            rocket.Launch(_stats.RocketDamage, _shards, _audio, _pool);
        }
    }
}
