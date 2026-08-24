using StackAttack.Core;
using VContainer;
using VContainer.Unity;

namespace StackAttack.Level
{
    public class LevelInstaller : IInstaller
    {
        private readonly LevelSet _levelSet;

        public LevelInstaller(LevelSet levelSet)
        {
            _levelSet = levelSet;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelSet);
            builder.RegisterInstance(new LevelProgress());
            builder.RegisterComponentInHierarchy<LevelRunner>();
            builder.RegisterComponentInHierarchy<BackgroundScroller>();
        }
    }
}
