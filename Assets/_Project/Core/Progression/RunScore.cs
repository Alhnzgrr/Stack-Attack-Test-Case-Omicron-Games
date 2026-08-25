using System;
using UnityEngine;

namespace StackAttack.Core
{
    public class RunScore
    {
        private readonly int _firstCost;
        private readonly int _costGrowth;

        private int _towardsNext;

        public RunScore(int firstCost, int costGrowth)
        {
            _firstCost = firstCost;
            _costGrowth = costGrowth;
            Multiplier = 1f;
        }

        public int Points { get; private set; }

        public int EarnedUpgrades { get; private set; }

        public int PendingUpgrades { get; private set; }

        public float Multiplier { get; set; }

        // Each upgrade costs more than the last, so the choices thin out over a level
        // instead of arriving in an accelerating flood near the end.
        public int NextCost => _firstCost + _costGrowth * EarnedUpgrades;

        public float Normalized => Mathf.Clamp01((float)_towardsNext / NextCost);

        // Points only move when a plate breaks, so the bar has something to listen
        // to instead of re-reading this every frame.
        public event Action Changed;

        public void Reset()
        {
            Points = 0;
            EarnedUpgrades = 0;
            PendingUpgrades = 0;
            _towardsNext = 0;
            Multiplier = 1f;
            Changed?.Invoke();
        }

        public void Add(int amount)
        {
            int scaled = Mathf.Max(Mathf.RoundToInt(amount * Multiplier), 1);

            Points += scaled;
            _towardsNext += scaled;

            // One award can cross more than one threshold, and the cost of the next
            // one rises as it does, so the crossings are drained in a loop.
            while (_towardsNext >= NextCost)
            {
                _towardsNext -= NextCost;
                EarnedUpgrades++;
                PendingUpgrades++;
            }

            Changed?.Invoke();
        }

        public bool TryConsume()
        {
            if (PendingUpgrades <= 0)
                return false;

            PendingUpgrades--;
            return true;
        }
    }
}
