using StackAttack.Core;
using VContainer;
using VContainer.Unity;

namespace StackAttack.Persistence
{
    public class PersistenceInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IProgressRepository, PlayerPrefsProgressRepository>(Lifetime.Singleton);
        }
    }
}
