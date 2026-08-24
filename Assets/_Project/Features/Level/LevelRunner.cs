using System;
using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Level
{
    public class LevelRunner : MonoBehaviour
    {
        private LevelSet _levelSet;
        private IStackSpawner _spawner;
        private LevelProgress _progress;
        private IProgressRepository _repository;

        private LevelConfig _config;
        private StackGroupEntry[] _entries;
        private int _nextEntry;
        private bool _completionSaved;

        [Inject]
        public void Construct(LevelSet levelSet, IStackSpawner spawner, LevelProgress progress, IProgressRepository repository)
        {
            _levelSet = levelSet;
            _spawner = spawner;
            _progress = progress;
            _repository = repository;
        }

        private void Start()
        {
            _progress.LevelIndex = Mathf.Clamp(_repository.LastLevelIndex, 0, _levelSet.Count - 1);
            _config = _levelSet.Get(_progress.LevelIndex);

            _entries = _config.Entries.ToArray();
            Array.Sort(_entries, LevelConfig.CompareByDistance);

            _progress.Length = _config.LevelLength;
            _progress.Travelled = 0f;
            _nextEntry = 0;
            _completionSaved = false;

            _spawner.Clear();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            if (deltaTime <= 0f)
                return;

            if (_progress.IsCompleted)
                SaveCompletionOnce();
            else
                Advance(deltaTime);

            _spawner.Tick();
        }

        private void Advance(float deltaTime)
        {
            _progress.Travelled += _config.ScrollSpeed * deltaTime;

            while (_nextEntry < _entries.Length && _entries[_nextEntry].distance <= _progress.Travelled)
            {
                _spawner.Spawn(_entries[_nextEntry], _config.ScrollSpeed);
                _nextEntry++;
            }
        }

        private void SaveCompletionOnce()
        {
            if (_completionSaved)
                return;

            _completionSaved = true;
            _repository.SaveLastLevelIndex(Mathf.Min(_progress.LevelIndex + 1, _levelSet.Count - 1));
        }
    }
}
