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
        private readonly ObjectPool<StackGroup> _pool;
        private readonly List<StackGroup> _active = new List<StackGroup>();

        public StackSpawner(StackGroup groupPrefab, PlayfieldConfig playfield, IScoreSink score)
        {
            _groupPrefab = groupPrefab;
            _playfield = playfield;
            _score = score;

            _pool = new ObjectPool<StackGroup>(
                Create,
                group => group.gameObject.SetActive(true),
                group => group.gameObject.SetActive(false),
                group => Object.Destroy(group.gameObject),
                true,
                8);
        }

        public void Spawn(StackGroupEntry entry, float descentSpeed)
        {
            StackGroup group = _pool.Get();

            group.Setup(entry, _playfield.HalfWidth, _playfield.SpawnY, descentSpeed, _score);
            _active.Add(group);
        }

        public void Tick()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (!_active[i].HasPassed(_playfield.DespawnY))
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
