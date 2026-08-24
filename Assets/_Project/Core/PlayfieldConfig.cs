using UnityEngine;

namespace StackAttack.Core
{
    [CreateAssetMenu(fileName = "PlayfieldConfig", menuName = "StackAttack/Core/Playfield Config")]
    public class PlayfieldConfig : ScriptableObject
    {
        [SerializeField] private float halfWidth = 2.5f;

        [SerializeField] private float spawnY = 7f;
        [SerializeField] private float despawnY = -7f;

        public float HalfWidth => halfWidth;
        public float SpawnY => spawnY;
        public float DespawnY => despawnY;
    }
}
