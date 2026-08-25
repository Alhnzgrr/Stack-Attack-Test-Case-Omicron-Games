using VContainer;
using VContainer.Unity;

namespace StackAttack.Screens
{
    public class ScreensInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<ScreenRouter>();
            builder.RegisterComponentInHierarchy<StartScreen>();
            builder.RegisterComponentInHierarchy<GameplayHud>();
            builder.RegisterComponentInHierarchy<EndScreen>();
            builder.RegisterComponentInHierarchy<UpgradeScreen>();
        }
    }
}
