using UnityEngine;

namespace StackAttack.Core
{
    [CreateAssetMenu(fileName = "PlayfieldConfig", menuName = "StackAttack/Core/Playfield Config")]
    public class PlayfieldConfig : ScriptableObject
    {
        [SerializeField] private float halfWidth = 2.5f;

        public float HalfWidth => halfWidth;
    }
}
