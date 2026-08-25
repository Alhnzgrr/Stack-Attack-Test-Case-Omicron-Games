using StackAttack.Core;
using VContainer;
using VContainer.Unity;

namespace StackAttack.Upgrades
{
    public class UpgradesInstaller : IInstaller
    {
        private readonly UpgradePool _pool;

        public UpgradesInstaller(UpgradePool pool)
        {
            _pool = pool;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_pool);
            builder.RegisterInstance(new RunScore(_pool.FirstCost, _pool.CostGrowth));
            builder.Register<UpgradeService>(Lifetime.Singleton).As<IUpgradeService, IScoreSink>();

            // The service resets the run from a state change, so it has to exist
            // before the first one fires rather than waiting to be resolved.
            builder.RegisterBuildCallback(resolver => resolver.Resolve<IUpgradeService>());
        }
    }
}
