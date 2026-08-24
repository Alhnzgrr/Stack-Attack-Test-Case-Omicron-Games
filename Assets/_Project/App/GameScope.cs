using StackAttack.Core;
using StackAttack.Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace StackAttack.App
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private PlayfieldConfig playfieldConfig;
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private WeaponConfig weaponConfig;
        [SerializeField] private HealthConfig healthConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(playfieldConfig);

            new PlayerInstaller(playerConfig, weaponConfig, healthConfig).Install(builder);
        }
    }
}
