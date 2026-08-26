using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    public class PlayerWeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private Projectile projectilePrefab;

        private IPointerInput _input;
        private GameStateMachine _state;
        private WeaponStats _stats;
        private ProjectileField _field;
        private AutoWeapon _weapon;

        [Inject]
        public void Construct(IPointerInput input, WeaponStats stats, ProjectileField field, GameStateMachine state)
        {
            _input = input;
            _state = state;
            _stats = stats;
            _field = field;
            _weapon = new AutoWeapon(stats);
        }

        private void Update()
        {
            if (_state.Current != GameState.Playing)
                return;

            if (_weapon.Tick(Time.deltaTime, _input.IsPressed))
                Fire();
        }

        private void Fire()
        {
            float size = _stats.ProjectileSize;
            float spread = (_stats.ProjectileCount - 1) * 0.5f * size;

            for (int i = 0; i < _stats.ProjectileCount; i++)
            {
                Vector3 position = muzzle.position + Vector3.right * (i * size - spread);
                Projectile projectile = Instantiate(projectilePrefab, position, Quaternion.identity, _field.Root);

                projectile.Launch(_stats.Damage, size, _stats.Pierce);
            }
        }
    }
}
