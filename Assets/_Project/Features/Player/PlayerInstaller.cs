using VContainer;
using VContainer.Unity;

namespace StackAttack.Player
{
    public class PlayerInstaller : IInstaller
    {
        private readonly PlayerConfig _config;

        public PlayerInstaller(PlayerConfig config)
        {
            _config = config;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_config);
            builder.Register<IDragInput, PointerDragInput>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<PlayerMovementBehaviour>();
        }
    }
}
