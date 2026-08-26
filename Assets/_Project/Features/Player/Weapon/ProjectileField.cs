using System;
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

        // Everything the player throws outlives the level that threw it, and one round
        // still in the air when the next level opens would hit its first stack. Three
        // launchers share this field so the call to sweep up is raised once, in one
        // place; each of them hands its own pool back what it still has out.
        public event Action Cleared;

        private void Start()
        {
            _state.Changed += OnStateChanged;
        }

        private void OnDestroy()
        {
            _state.Changed -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            Cleared?.Invoke();
        }
    }
}
