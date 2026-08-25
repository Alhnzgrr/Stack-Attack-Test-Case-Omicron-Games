using System;
using UnityEngine;

namespace StackAttack.Core
{
    [Serializable]
    public class UpgradeOption
    {
        public UpgradeKind kind;
        public string category = "BALL";
        public string statName;
        public float amount = 1f;
        public Sprite icon;
        public Color tint = Color.white;

        // Which kinds read as a percentage is a property of the kind itself, so the
        // label cannot drift away from how the upgrade is actually applied.
        public bool IsPercent =>
            kind == UpgradeKind.FireRate ||
            kind == UpgradeKind.ProjectileSize ||
            kind == UpgradeKind.Greed;

        public string ValueLabel => IsPercent
            ? string.Format("+{0:0}%", amount * 100f)
            : string.Format("+{0:0}", amount);
    }
}
