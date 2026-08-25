using StackAttack.Core;
using VContainer;
using VContainer.Unity;

namespace StackAttack.StackEnemy
{
    public class StackEnemyInstaller : IInstaller
    {
        private readonly StackGroup _groupPrefab;

        public StackEnemyInstaller(StackGroup groupPrefab)
        {
            _groupPrefab = groupPrefab;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<ShardBurst>().As<IShardBurst>();
            builder.Register<IStackSpawner, StackSpawner>(Lifetime.Singleton)
                .WithParameter(_groupPrefab);
        }
    }
}
