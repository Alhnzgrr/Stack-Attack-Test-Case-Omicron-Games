using StackAttack.Core;
using VContainer;
using VContainer.Unity;

namespace StackAttack.Audio
{
    public class AudioInstaller : IInstaller
    {
        private readonly AudioBank _bank;

        public AudioInstaller(AudioBank bank)
        {
            _bank = bank;
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<AudioPlayer>();
            builder.Register<IAudioService, AudioService>(Lifetime.Singleton)
                .WithParameter(_bank);
        }
    }
}
