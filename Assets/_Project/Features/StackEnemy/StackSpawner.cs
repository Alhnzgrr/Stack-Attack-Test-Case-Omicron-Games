using System.Collections.Generic;
using StackAttack.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace StackAttack.StackEnemy
{
    public class StackSpawner : IStackSpawner
    {
        private readonly StackGroup _groupPrefab;
        private readonly PlayfieldConfig _playfield;
        private readonly IScoreSink _score;
        private readonly IShardBurst _shards;
        private readonly IAudioService _audio;
        private readonly ObjectPool<StackGroup> _pool;
        private readonly List<StackGroup> _active = new List<StackGroup>();

        public StackSpawner(StackGroup groupPrefab, PlayfieldConfig playfield, IScoreSink score, IShardBurst shards, IAudioService audio)
        {
            _groupPrefab = groupPrefab;
            _playfield = playfield;
            _score = score;
            _shards = shards;
            _audio = audio;

            _pool = new ObjectPool<StackGroup>(
                Create,
                group => group.gameObject.SetActive(true),
                group => group.gameObject.SetActive(false),
                group => Object.Destroy(group.gameObject),
                true,
                8);
        }

        public bool HasActiveGroups => _active.Count > 0;

        public void Spawn(StackGroupEntry entry, float descentSpeed)
        {
            SpawnAt(entry, descentSpeed, _playfield.SpawnY);
        }

        // A level pushes its groups in from above the screen, but a boss throws them
        // from wherever it is standing, so the line they start on is the caller's.
        public void SpawnAt(StackGroupEntry entry, float descentSpeed, float spawnY)
        {
            StackGroup group = _pool.Get();

            group.Setup(entry, _playfield.HalfWidth, spawnY, descentSpeed, _score, _shards, _audio);
            _active.Add(group);
        }

        public void Tick()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (!_active[i].HasPassed(_playfield.DespawnY) && !_active[i].IsCleared)
                    continue;

                _pool.Release(_active[i]);
                _active.RemoveAt(i);
            }
        }

        public void Clear()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
                _pool.Release(_active[i]);

            _active.Clear();
        }

        private StackGroup Create()
        {
            return Object.Instantiate(_groupPrefab);
        }
    }
}
