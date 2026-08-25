using System.Collections.Generic;
using StackAttack.Core;
using UnityEngine;

namespace StackAttack.Upgrades
{
    [CreateAssetMenu(fileName = "UpgradePool", menuName = "StackAttack/Upgrades/Upgrade Pool")]
    public class UpgradePool : ScriptableObject
    {
        [SerializeField] private int offerSize = 3;
        [SerializeField] private int firstCost = 10;
        [SerializeField] private int costGrowth = 8;

        // The defaults are written here rather than left blank so a freshly created
        // asset is already a playable pool the designer can tune instead of fill.
        [SerializeField] private List<UpgradeOption> options = new List<UpgradeOption>
        {
            new UpgradeOption
            {
                kind = UpgradeKind.FireRate,
                title = "Rapid Fire",
                description = "+2 shots per second",
                amount = 2f,
                tint = new Color(1f, 0.78f, 0.2f, 1f)
            },
            new UpgradeOption
            {
                kind = UpgradeKind.Damage,
                title = "Heavy Rounds",
                description = "+1 damage per shot",
                amount = 1f,
                tint = new Color(0.95f, 0.32f, 0.28f, 1f)
            },
            new UpgradeOption
            {
                kind = UpgradeKind.ProjectileCount,
                title = "Fork Shot",
                description = "+1 projectile",
                amount = 1f,
                tint = new Color(0.36f, 0.72f, 1f, 1f)
            },
            new UpgradeOption
            {
                kind = UpgradeKind.ProjectileSize,
                title = "Wide Shot",
                description = "+40% projectile size",
                amount = 0.4f,
                tint = new Color(0.45f, 0.85f, 0.6f, 1f)
            },
            new UpgradeOption
            {
                kind = UpgradeKind.Pierce,
                title = "Piercing",
                description = "Shots punch through one more stack",
                amount = 1f,
                tint = new Color(0.72f, 0.5f, 1f, 1f)
            },
            new UpgradeOption
            {
                kind = UpgradeKind.MaxHealth,
                title = "Reinforce",
                description = "+1 heart",
                amount = 1f,
                tint = new Color(1f, 0.45f, 0.65f, 1f)
            },
            new UpgradeOption
            {
                kind = UpgradeKind.Greed,
                title = "Greed",
                description = "+25% points from plates",
                amount = 0.25f,
                tint = new Color(0.85f, 0.8f, 0.35f, 1f)
            }
        };

        public int OfferSize => offerSize;
        public int FirstCost => firstCost;
        public int CostGrowth => costGrowth;
        public List<UpgradeOption> Options => options;
    }
}
