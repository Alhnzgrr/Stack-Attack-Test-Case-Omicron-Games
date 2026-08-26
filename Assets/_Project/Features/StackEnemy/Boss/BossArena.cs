using StackAttack.Core;
using UnityEngine;

namespace StackAttack.StackEnemy
{
    public class BossArena : IBossArena
    {
        private readonly BossBehaviour _bossPrefab;
        private readonly PlayfieldConfig _playfield;
        private readonly IStackSpawner _spawner;
        private readonly IScoreSink _score;
        private readonly IShardBurst _shards;
        private readonly IAudioService _audio;

        private BossBehaviour _boss;

        public BossArena(BossBehaviour bossPrefab, PlayfieldConfig playfield, IStackSpawner spawner, IScoreSink score, IShardBurst shards, IAudioService audio)
        {
            _bossPrefab = bossPrefab;
            _playfield = playfield;
            _spawner = spawner;
            _score = score;
            _shards = shards;
            _audio = audio;
        }

        // The boss takes itself off the field when it dies, so the destroyed
        // reference is what says the fight is over.
        public bool IsActive => _boss != null;

        public void Begin(StackGroupEntry entry)
        {
            Clear();

            _boss = Object.Instantiate(_bossPrefab);
            _boss.Setup(entry, _playfield, _spawner, _score, _shards, _audio);
        }

        public void Clear()
        {
            if (_boss == null)
                return;

            Object.Destroy(_boss.gameObject);
            _boss = null;
        }
    }
}
