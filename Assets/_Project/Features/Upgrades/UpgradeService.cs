using System;
using System.Collections.Generic;
using StackAttack.Core;
using UnityEngine;

namespace StackAttack.Upgrades
{
    public class UpgradeService : IUpgradeService, IScoreSink, IDisposable
    {
        private readonly UpgradePool _pool;
        private readonly RunScore _score;
        private readonly WeaponStats _stats;
        private readonly PlayerHealth _health;
        private readonly GameStateMachine _state;

        private readonly List<UpgradeOption> _offer = new List<UpgradeOption>();
        private readonly List<UpgradeOption> _candidates = new List<UpgradeOption>();

        public UpgradeService(
            UpgradePool pool,
            RunScore score,
            WeaponStats stats,
            PlayerHealth health,
            GameStateMachine state)
        {
            _pool = pool;
            _score = score;
            _stats = stats;
            _health = health;
            _state = state;

            _state.Changed += OnStateChanged;
        }

        public bool IsChoosing { get; private set; }

        public IReadOnlyList<UpgradeOption> Offer => _offer;

        public event Action Changed;

        public void Dispose()
        {
            _state.Changed -= OnStateChanged;
            Time.timeScale = 1f;
        }

        public void AddPoints(int amount)
        {
            _score.Add(amount);

            // A threshold crossed while a card is already up stays queued; the next
            // offer rolls the moment this one is answered.
            if (IsChoosing)
                return;

            if (_score.TryConsume())
                Roll();
        }

        public void Choose(int index)
        {
            if (!IsChoosing || index < 0 || index >= _offer.Count)
                return;

            Apply(_offer[index]);

            if (_score.TryConsume())
            {
                Roll();
                return;
            }

            IsChoosing = false;
            Time.timeScale = 1f;
            Changed?.Invoke();
        }

        private void OnStateChanged(GameState state)
        {
            Time.timeScale = 1f;
            IsChoosing = false;
            _offer.Clear();
            Changed?.Invoke();

            if (state != GameState.Playing)
                return;

            // Upgrades are run-scoped: every level starts from the configured base so
            // a later level cannot be trivialised by what an earlier one handed out.
            _score.Reset();
            _stats.ResetToBase();
            _health.ResetToBase();
        }

        private void Roll()
        {
            _offer.Clear();
            _candidates.Clear();
            _candidates.AddRange(_pool.Options);

            while (_offer.Count < _pool.OfferSize && _candidates.Count > 0)
            {
                UpgradeOption picked = _candidates[UnityEngine.Random.Range(0, _candidates.Count)];
                _offer.Add(picked);

                // Two cards feeding the same stat would waste a choice, so a kind
                // leaves the pool as soon as it is offered.
                for (int i = _candidates.Count - 1; i >= 0; i--)
                {
                    if (_candidates[i].kind == picked.kind)
                        _candidates.RemoveAt(i);
                }
            }

            IsChoosing = true;

            // Freezing time rather than routing through the state machine keeps the
            // choice a pause inside the level instead of a fourth game state.
            Time.timeScale = 0f;
            Changed?.Invoke();
        }

        private void Apply(UpgradeOption option)
        {
            switch (option.kind)
            {
                case UpgradeKind.FireRate:
                    _stats.FireRate *= 1f + option.amount;
                    break;
                case UpgradeKind.Damage:
                    _stats.Damage += option.amount;
                    break;
                case UpgradeKind.ProjectileCount:
                    _stats.ProjectileCount += Mathf.RoundToInt(option.amount);
                    break;
                case UpgradeKind.ProjectileSize:
                    _stats.ProjectileSize *= 1f + option.amount;
                    break;
                case UpgradeKind.Pierce:
                    _stats.Pierce += Mathf.RoundToInt(option.amount);
                    break;
                case UpgradeKind.MaxHealth:
                    _health.Grow(Mathf.RoundToInt(option.amount));
                    break;
                case UpgradeKind.Greed:
                    _score.Multiplier += option.amount;
                    break;
                case UpgradeKind.BoomerangCount:
                    _stats.BoomerangCount += Mathf.RoundToInt(option.amount);
                    break;
                case UpgradeKind.RocketDamage:
                    _stats.RocketDamage += option.amount;
                    break;
            }
        }
    }
}
