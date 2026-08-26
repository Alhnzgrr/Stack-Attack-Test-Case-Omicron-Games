using System;
using System.Collections.Generic;
using StackAttack.Core;
using UnityEngine;

namespace StackAttack.Audio
{
    [CreateAssetMenu(fileName = "AudioBank", menuName = "StackAttack/Audio/Audio Bank")]
    public class AudioBank : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public GameSound sound;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;

            // The same sample fired fifteen times a second turns into a drone. A few
            // percent either side of the recorded pitch is what keeps it a gunshot.
            [Range(0f, 0.5f)] public float pitchJitter = 0.05f;
        }

        // Every sound the game asks for is listed here with no clip attached, so the
        // bank arrives as a checklist rather than something to remember to fill.
        [SerializeField] private List<Entry> entries = new List<Entry>
        {
            new Entry { sound = GameSound.Shot, volume = 0.5f, pitchJitter = 0.06f },
            new Entry { sound = GameSound.PlateBreak, volume = 0.8f, pitchJitter = 0.08f },
            new Entry { sound = GameSound.PlayerHit, volume = 1f, pitchJitter = 0.03f },
            new Entry { sound = GameSound.UpgradeOffer, volume = 0.9f, pitchJitter = 0f },
            new Entry { sound = GameSound.UpgradeTaken, volume = 0.9f, pitchJitter = 0f },
            new Entry { sound = GameSound.RocketBlast, volume = 0.9f, pitchJitter = 0.05f },
            new Entry { sound = GameSound.BoomerangThrow, volume = 0.7f, pitchJitter = 0.05f },
            new Entry { sound = GameSound.BossArrive, volume = 1f, pitchJitter = 0f },
            new Entry { sound = GameSound.BossHit, volume = 0.7f, pitchJitter = 0.08f },
            new Entry { sound = GameSound.BossDie, volume = 1f, pitchJitter = 0f },
            new Entry { sound = GameSound.LevelWon, volume = 1f, pitchJitter = 0f },
            new Entry { sound = GameSound.LevelLost, volume = 1f, pitchJitter = 0f }
        };

        public IReadOnlyList<Entry> Entries => entries;
    }
}
