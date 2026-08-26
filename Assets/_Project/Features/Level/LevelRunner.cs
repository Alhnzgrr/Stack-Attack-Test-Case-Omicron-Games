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
        private IBossArena _boss;
        private LevelProgress _progress;
        private RunScore _score;
        private IProgressRepository _repository;
        private GameStateMachine _state;

        private LevelConfig _config;
        private StackGroupEntry[] _entries;
        private int _nextEntry;

        [Inject]
        public void Construct(
            LevelSet levelSet,
            IStackSpawner spawner,
            IBossArena boss,
            LevelProgress progress,
            RunScore score,
            IProgressRepository repository,
            GameStateMachine state)
        {
            _levelSet = levelSet;
            _spawner = spawner;
            _boss = boss;
            _progress = progress;
            _score = score;
            _repository = repository;
            _state = state;
        }

        private void Start()
        {
            _state.Changed += OnStateChanged;
            _progress.LevelIndex = Mathf.Clamp(_repository.LastLevelIndex, 0, _levelSet.Count - 1);
        }

        private void OnDestroy()
        {
            _state.Changed -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Playing)
            {
                Begin();
                return;
            }

            // Nothing ticks the spawner outside a run, so anything left on the field
            // would drift on forever behind the screen that just came up.
            _spawner.Clear();
            _boss.Clear();
        }

        private void Begin()
        {
            _progress.LevelIndex = Mathf.Clamp(_repository.LastLevelIndex, 0, _levelSet.Count - 1);
            _config = _levelSet.Get(_progress.LevelIndex);

            _entries = _config.Entries.ToArray();
            Array.Sort(_entries, LevelConfig.CompareByDistance);

            _score.CostScale = _config.UpgradeCostScale;

            _progress.Length = _config.LevelLength;
            _progress.Travelled = 0f;
            _nextEntry = 0;

            _spawner.Clear();
            _boss.Clear();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            if (deltaTime <= 0f || _state.Current != GameState.Playing)
                return;

            Advance(deltaTime);
            _spawner.Tick();

            // The bar only measures how far the level has scrolled. Once the last
            // entry is out there is nothing left to wait for but the playfield, so an
            // empty field ends the level rather than the clock running down.
            if (_nextEntry >= _entries.Length && !_spawner.HasActiveGroups && !_boss.IsActive)
                Win();
        }

        private void Advance(float deltaTime)
        {
            _progress.Travelled += _config.ScrollSpeed * deltaTime;

            while (_nextEntry < _entries.Length && _entries[_nextEntry].distance <= _progress.Travelled)
            {
                StackGroupEntry entry = _entries[_nextEntry];

                // A boss entry stops where the level put it instead of scrolling
                // through, so the arena takes it rather than the spawner.
                if (entry.boss != null)
                    _boss.Begin(entry);
                else
                    _spawner.Spawn(entry, _config.ScrollSpeed);

                _nextEntry++;
            }
        }

        private void Win()
        {
            // The level is over early by design, so the bar is filled rather than
            // left short of the end it never had to reach.
            _progress.Travelled = _progress.Length;

            _repository.SaveLastLevelIndex(Mathf.Min(_progress.LevelIndex + 1, _levelSet.Count - 1));
            _state.Set(GameState.Won);
        }
    }
}
