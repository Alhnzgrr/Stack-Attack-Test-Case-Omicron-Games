using StackAttack.Core;
using VContainer;
using VContainer.Unity;

namespace StackAttack.Level
{
    public class LevelInstaller : IInstaller
    {
        private readonly LevelConfig _config;

        public LevelInstaller(LevelConfig config)
        {
            _config = config;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_config);
            builder.RegisterInstance(new LevelProgress());
            builder.RegisterComponentInHierarchy<LevelRunner>();
        }
    }
}
