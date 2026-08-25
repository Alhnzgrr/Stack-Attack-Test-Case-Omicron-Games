using System;
using UnityEngine;

namespace StackAttack.Core
{
    [Serializable]
    public class UpgradeOption
    {
        public UpgradeKind kind;
        public string title;
        public string description;
        public float amount = 1f;
        public Color tint = Color.white;
    }
}
