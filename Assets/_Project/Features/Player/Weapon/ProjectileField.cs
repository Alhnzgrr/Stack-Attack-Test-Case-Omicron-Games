using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    public class ProjectileField : MonoBehaviour
    {
        private GameStateMachine _state;

        [Inject]
        public void Construct(GameStateMachine state)
        {
            _state = state;
        }

        public Transform Root => transform;

        private void Start()
        {
            _state.Changed += Clear;
        }

        private void OnDestroy()
        {
            _state.Changed -= Clear;
        }

        // Everything the player throws outlives the level that threw it, and one round
        // still in the air when the next level opens would hit its first stack. Three
        // launchers share this field so the sweeping up happens once, in one place.
        private void Clear(GameState state)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}
