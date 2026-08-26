using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    public class PlayerBoomerangBehaviour : MonoBehaviour
    {
        [SerializeField] private Boomerang boomerangPrefab;
        [SerializeField] private float damage = 3f;

        private GameStateMachine _state;
        private WeaponStats _stats;
        private ProjectileField _field;
        private SkillTimer _timer;

        [Inject]
        public void Construct(WeaponStats stats, ProjectileField field, GameStateMachine state)
        {
            _stats = stats;
            _field = field;
            _state = state;
            _timer = new SkillTimer(stats.BoomerangInterval);
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
            float step = Mathf.PI * 2f / _stats.BoomerangCount;

            for (int i = 0; i < _stats.BoomerangCount; i++)
            {
                Boomerang boomerang = Instantiate(boomerangPrefab, transform.position, Quaternion.identity, _field.Root);

                boomerang.Launch(transform, damage, step * i);
            }
        }
    }
}
