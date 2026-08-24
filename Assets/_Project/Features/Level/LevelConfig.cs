using System.Collections.Generic;
using StackAttack.Core;
using UnityEngine;

namespace StackAttack.Level
{
    [CreateAssetMenu(fileName = "Level", menuName = "StackAttack/Level/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private float levelLength = 100f;
        [SerializeField] private float scrollSpeed = 3f;
        [SerializeField] private List<StackGroupEntry> entries = new List<StackGroupEntry>();

        public float LevelLength => levelLength;
        public float ScrollSpeed => scrollSpeed;
        public List<StackGroupEntry> Entries => entries;

        // Sorting while the entries are being typed would reorder the list on every
        // keystroke, so it is an explicit action instead.
        [ContextMenu("Sort Entries By Distance")]
        private void SortEntriesByDistance()
        {
            entries.Sort(CompareByDistance);
        }

        public static int CompareByDistance(StackGroupEntry left, StackGroupEntry right)
        {
            return left.distance.CompareTo(right.distance);
        }
    }
}
