using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    public class PlayerBoomerangBehaviour : MonoBehaviour
    {
        [SerializeField] private Boomerang boomerangPrefab;
        [SerializeField] private float damage = 3f;
        [SerializeField] private int poolCapacity = 8;

        private GameStateMachine _state;
        private WeaponStats _stats;
        private ProjectileField _field;
        private IAudioService _audio;
        private SkillTimer _timer;
        private ThrownPool<Boomerang> _pool;

        [Inject]
        public void Construct(WeaponStats stats, ProjectileField field, IAudioService audio, GameStateMachine state)
        {
            _stats = stats;
            _field = field;
            _audio = audio;
            _state = state;
            _timer = new SkillTimer(stats.BoomerangInterval);
            _pool = new ThrownPool<Boomerang>(boomerangPrefab, field.Root, poolCapacity);
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

            // The clock runs whether or not a card has been taken, so the first throw
            // lands on the same beat every later one does.
            if (_stats.BoomerangCount <= 0)
                return;

            Throw();
        }

        // Extra boomerangs share one throw rather than queueing behind it: spacing the
        // starting angle evenly sends them around opposite sides of the same arc.
        private void Throw()
        {
            _audio.Play(GameSound.BoomerangThrow);

            float step = Mathf.PI * 2f / _stats.BoomerangCount;

            for (int i = 0; i < _stats.BoomerangCount; i++)
            {
                Boomerang boomerang = _pool.Take(transform.position);

                boomerang.Launch(transform, damage, step * i, _pool);
            }
        }
    }
}
