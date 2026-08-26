using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    public class PlayerRocketBehaviour : MonoBehaviour
    {
        [SerializeField] private Rocket rocketPrefab;

        private GameStateMachine _state;
        private WeaponStats _stats;
        private ProjectileField _field;
        private IShardBurst _shards;
        private SkillTimer _timer;

        [Inject]
        public void Construct(WeaponStats stats, ProjectileField field, IShardBurst shards, GameStateMachine state)
        {
            _stats = stats;
            _field = field;
            _shards = shards;
            _state = state;
            _timer = new SkillTimer(stats.RocketInterval);
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

            Rocket rocket = Instantiate(rocketPrefab, transform.position, Quaternion.identity, _field.Root);

            rocket.Launch(_stats.RocketDamage, _shards);
        }
    }
}
