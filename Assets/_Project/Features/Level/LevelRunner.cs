using System;
using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Level
{
    public class LevelRunner : MonoBehaviour
    {
        private LevelConfig _config;
        private IStackSpawner _spawner;
        private LevelProgress _progress;

        private StackGroupEntry[] _entries;
        private int _nextEntry;

        [Inject]
        public void Construct(LevelConfig config, IStackSpawner spawner, LevelProgress progress)
        {
            _config = config;
            _spawner = spawner;
            _progress = progress;
        }

        private void Start()
        {
            _entries = _config.Entries.ToArray();
            Array.Sort(_entries, LevelConfig.CompareByDistance);

            _progress.Length = _config.LevelLength;
            _progress.Travelled = 0f;
            _nextEntry = 0;

            _spawner.Clear();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            if (deltaTime <= 0f)
                return;

            if (!_progress.IsCompleted)
            {
                _progress.Travelled += _config.ScrollSpeed * deltaTime;
                SpawnDueEntries();
            }

            _spawner.Tick();
        }

        private void SpawnDueEntries()
        {
            while (_nextEntry < _entries.Length && _entries[_nextEntry].distance <= _progress.Travelled)
            {
                _spawner.Spawn(_entries[_nextEntry], _config.ScrollSpeed);
                _nextEntry++;
            }
        }
    }
}
