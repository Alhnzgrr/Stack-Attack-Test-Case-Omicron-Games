using StackAttack.Core;
using VContainer;
using VContainer.Unity;

namespace StackAttack.Player
{
    public class PlayerInstaller : IInstaller
    {
        private readonly PlayerConfig _playerConfig;
        private readonly WeaponConfig _weaponConfig;
        private readonly HealthConfig _healthConfig;

        public PlayerInstaller(PlayerConfig playerConfig, WeaponConfig weaponConfig, HealthConfig healthConfig)
        {
            _playerConfig = playerConfig;
            _weaponConfig = weaponConfig;
            _healthConfig = healthConfig;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_playerConfig);
            builder.RegisterInstance(_healthConfig);
            builder.RegisterInstance(_weaponConfig.CreateStats());
            builder.RegisterInstance(new PlayerHealth(_healthConfig.MaxHealth, _healthConfig.InvulnerabilityDuration));
            builder.Register<IPointerInput, PointerInput>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<PlayerMovementBehaviour>();
            builder.RegisterComponentInHierarchy<PlayerWeaponBehaviour>();
            builder.RegisterComponentInHierarchy<PlayerHealthBehaviour>();
        }
    }
}
