using StackAttack.Core;
using TMPro;
using UnityEngine;
using VContainer;

namespace StackAttack.Screens
{
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI hintLabel;

        private IPointerInput _input;
        private GameStateMachine _state;

        [Inject]
        public void Construct(IPointerInput input, GameStateMachine state)
        {
            _input = input;
            _state = state;
        }

        private void OnEnable()
        {
            bool won = _state.Current == GameState.Won;

            titleLabel.SetText(won ? "Level Complete" : "Game Over");
            hintLabel.SetText(won ? "Tap for the next level" : "Tap to try again");
        }

        private void Update()
        {
            if (_input.PressedThisFrame)
                _state.Set(GameState.Playing);
        }
    }
}
