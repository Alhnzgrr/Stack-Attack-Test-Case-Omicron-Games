using UnityEngine;

namespace StackAttack.Core
{
    public static class StackGroupGeometry
    {
        public static int MemberCount(StackGroupEntry entry)
        {
            return entry.layout == GroupLayout.Single ? 1 : Mathf.Max(entry.count, 1);
        }

        public static int PlateCount(StackGroupEntry entry)
        {
            if (entry.stackType == null || entry.stackType.HitsPerPlate <= 0)
                return 0;

            return Mathf.CeilToInt(entry.hp / (float)entry.stackType.HitsPerPlate);
        }

        public static float StackHeight(StackGroupEntry entry)
        {
            if (entry.stackType == null)
                return 0f;

            return PlateCount(entry) * entry.stackType.PlateStep;
        }

        public static float HalfWidth(StackGroupEntry entry)
        {
            if (entry.stackType == null)
                return 0f;

            float half = entry.stackType.PlateSize.x * 0.5f;

            if (entry.layout == GroupLayout.Row)
                return (MemberCount(entry) - 1) * 0.5f * entry.spacing + half;

            if (entry.layout == GroupLayout.Ring)
                return entry.radius + half;

            return half;
        }
    }
}
