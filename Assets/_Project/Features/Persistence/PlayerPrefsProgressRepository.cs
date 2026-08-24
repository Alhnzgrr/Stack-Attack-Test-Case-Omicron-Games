using StackAttack.Core;
using UnityEngine;

namespace StackAttack.Persistence
{
    public class PlayerPrefsProgressRepository : IProgressRepository
    {
        private const string LastLevelKey = "stackattack.last_level_index";

        public int LastLevelIndex => PlayerPrefs.GetInt(LastLevelKey, 0);

        public void SaveLastLevelIndex(int index)
        {
            PlayerPrefs.SetInt(LastLevelKey, index);
            PlayerPrefs.Save();
        }
    }
}
