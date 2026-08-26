using StackAttack.Core;
using VContainer;
using VContainer.Unity;

namespace StackAttack.StackEnemy
{
    public class StackEnemyInstaller : IInstaller
    {
        private readonly StackGroup _groupPrefab;
        private readonly BossBehaviour _bossPrefab;

        public StackEnemyInstaller(StackGroup groupPrefab, BossBehaviour bossPrefab)
        {
            _groupPrefab = groupPrefab;
            _bossPrefab = bossPrefab;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<ShardBurst>().As<IShardBurst>();
            builder.Register<IStackSpawner, StackSpawner>(Lifetime.Singleton)
                .WithParameter(_groupPrefab);
            builder.Register<IBossArena, BossArena>(Lifetime.Singleton)
                .WithParameter(_bossPrefab);
        }
    }
}
