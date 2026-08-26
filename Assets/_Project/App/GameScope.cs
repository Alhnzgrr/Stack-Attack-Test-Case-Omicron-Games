using StackAttack.Audio;
using StackAttack.Core;
using StackAttack.Level;
using StackAttack.Persistence;
using StackAttack.Player;
using StackAttack.Playfield;
using StackAttack.Screens;
using StackAttack.StackEnemy;
using StackAttack.Upgrades;
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
        [SerializeField] private LevelSet levelSet;
        [SerializeField] private UpgradePool upgradePool;
        [SerializeField] private AudioBank audioBank;
        [SerializeField] private StackGroup stackGroupPrefab;
        [SerializeField] private BossBehaviour bossPrefab;
        [SerializeField] private int targetFrameRate = 60;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(playfieldConfig);
            builder.RegisterInstance(new GameStateMachine());
            builder.RegisterEntryPoint<ApplicationBootstrap>().WithParameter(targetFrameRate);

            new AudioInstaller(audioBank).Install(builder);
            new PlayerInstaller(playerConfig, weaponConfig, healthConfig).Install(builder);
            new PlayfieldInstaller().Install(builder);
            new UpgradesInstaller(upgradePool).Install(builder);
            new StackEnemyInstaller(stackGroupPrefab, bossPrefab).Install(builder);
            new PersistenceInstaller().Install(builder);
            new LevelInstaller(levelSet).Install(builder);
            new ScreensInstaller().Install(builder);
        }
    }
}
