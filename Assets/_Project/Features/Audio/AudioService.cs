using System.Collections.Generic;
using StackAttack.Core;
using UnityEngine;

namespace StackAttack.Audio
{
    public class AudioService : IAudioService
    {
        private readonly AudioPlayer _player;
        private readonly Dictionary<GameSound, AudioBank.Entry> _entries = new Dictionary<GameSound, AudioBank.Entry>();
        private readonly Dictionary<GameSound, int> _lastFrame = new Dictionary<GameSound, int>();

        public AudioService(AudioBank bank, AudioPlayer player)
        {
            _player = player;

            for (int i = 0; i < bank.Entries.Count; i++)
                _entries[bank.Entries[i].sound] = bank.Entries[i];
        }

        public void Play(GameSound sound)
        {
            // A bank the designer has not finished filling is a normal state to be in,
            // not a wiring bug, so a sound with no clip behind it is simply silent.
            if (!_entries.TryGetValue(sound, out AudioBank.Entry entry) || entry.clip == null)
                return;

            // One pierced shot can break three plates in the same frame. Three copies
            // of one sample on top of each other is not three times as loud, it is a
            // click, so the same sound is only allowed one voice per frame.
            if (_lastFrame.TryGetValue(sound, out int frame) && frame == Time.frameCount)
                return;

            _lastFrame[sound] = Time.frameCount;

            float pitch = 1f + Random.Range(-entry.pitchJitter, entry.pitchJitter);

            _player.Play(entry.clip, entry.volume, pitch);
        }
    }
}
