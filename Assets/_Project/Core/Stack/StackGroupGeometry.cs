using UnityEngine;

namespace StackAttack.Core
{
    public static class StackGroupGeometry
    {
        public static int MemberCount(StackGroupEntry entry)
        {
            return entry.layout == GroupLayout.Single ? 1 : Mathf.Max(entry.count, 1);
        }

        // What the hp asks for, before the stack runs out of plates to show.
        public static int RequestedPlateCount(StackGroupEntry entry)
        {
            if (entry.stackType == null || entry.stackType.HitsPerPlate <= 0)
                return 0;

            return Mathf.CeilToInt(entry.hp / (float)entry.stackType.HitsPerPlate);
        }

        public static int PlateCount(StackGroupEntry entry)
        {
            if (entry.stackType == null)
                return 0;

            return Mathf.Min(RequestedPlateCount(entry), entry.stackType.MaxPlates);
        }

        // Damage past this point cannot break a plate, so it scores nothing and
        // throws no shards. It is the ceiling a level has to be written under.
        public static int MaxHp(StackGroupEntry entry)
        {
            if (entry.stackType == null)
                return 0;

            return entry.stackType.MaxPlates * entry.stackType.HitsPerPlate;
        }

        public static bool IsCapped(StackGroupEntry entry)
        {
            return entry.stackType != null && RequestedPlateCount(entry) > entry.stackType.MaxPlates;
        }

        public static float StackHeight(StackGroupEntry entry)
        {
            if (entry.stackType == null)
                return 0f;

            // Plates step upward from the bottom one, which still has its own body,
            // so the silhouette is one plate taller than the stepping alone.
            return entry.stackType.PlateSize.y + Mathf.Max(PlateCount(entry) - 1, 0) * entry.stackType.PlateStep;
        }

        public static float HalfWidth(StackGroupEntry entry)
        {
            if (entry.stackType == null)
                return 0f;

            float half = entry.stackType.PlateSize.x * 0.5f;

            if (entry.layout == GroupLayout.Row)
                return (MemberCount(entry) - 1) * 0.5f * entry.spacing + half;

            if (entry.layout == GroupLayout.Ring || entry.layout == GroupLayout.Cluster)
                return entry.radius + half;

            return half;
        }

        public static Vector2 MemberOffset(StackGroupEntry entry, int index, int count)
        {
            if (entry.layout == GroupLayout.Row)
                return new Vector2(index * entry.spacing - (count - 1) * 0.5f * entry.spacing, 0f);

            if (entry.layout == GroupLayout.Ring)
                return OnCircle(index, count, entry.radius);

            // A cluster keeps its first member in the middle and closes the rest in
            // around it, so an orbit spins the ring while the core stays put.
            if (entry.layout == GroupLayout.Cluster && index > 0)
                return OnCircle(index - 1, Mathf.Max(count - 1, 1), entry.radius);

            return Vector2.zero;
        }

        private static Vector2 OnCircle(int index, int count, float radius)
        {
            float radians = 360f / count * index * Mathf.Deg2Rad;

            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
        }
    }
}
