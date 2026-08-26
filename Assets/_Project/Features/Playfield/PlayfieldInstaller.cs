using VContainer;
using VContainer.Unity;

namespace StackAttack.Playfield
{
    public class PlayfieldInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<PlayfieldCamera>();
        }
    }
}
