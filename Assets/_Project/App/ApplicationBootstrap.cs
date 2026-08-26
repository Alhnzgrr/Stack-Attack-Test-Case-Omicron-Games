using UnityEngine;
using VContainer.Unity;

namespace StackAttack.App
{
    public class ApplicationBootstrap : IStartable
    {
        private readonly int _targetFrameRate;

        public ApplicationBootstrap(int targetFrameRate)
        {
            _targetFrameRate = targetFrameRate;
        }

        public void Start()
        {
            // Mobile ignores vSync entirely, but the editor and desktop honour it and
            // would cap the rate before targetFrameRate is ever consulted, so testing
            // in the editor would not match the device.
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _targetFrameRate;

            // The level keeps running while the player only reacts, so a hands-off
            // stretch is not the idle device the OS would read it as.
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
