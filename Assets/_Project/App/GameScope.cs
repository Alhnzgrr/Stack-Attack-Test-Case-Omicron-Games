using StackAttack.Core;
using StackAttack.Level;
using StackAttack.Player;
using StackAttack.StackEnemy;
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
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private StackGroup stackGroupPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(playfieldConfig);

            new PlayerInstaller(playerConfig, weaponConfig, healthConfig).Install(builder);
            new StackEnemyInstaller(stackGroupPrefab).Install(builder);
            new LevelInstaller(levelConfig).Install(builder);
        }
    }
}
