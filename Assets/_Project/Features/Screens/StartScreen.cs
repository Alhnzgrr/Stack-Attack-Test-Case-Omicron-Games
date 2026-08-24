using StackAttack.Core;
using TMPro;
using UnityEngine;
using VContainer;

namespace StackAttack.Screens
{
    public class StartScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelLabel;

        private IPointerInput _input;
        private IProgressRepository _repository;
        private GameStateMachine _state;

        [Inject]
        public void Construct(IPointerInput input, IProgressRepository repository, GameStateMachine state)
        {
            _input = input;
            _repository = repository;
            _state = state;
        }

        private void OnEnable()
        {
            levelLabel.SetText("Level {0:0}", _repository.LastLevelIndex + 1);
        }

        private void Update()
        {
            if (_input.PressedThisFrame)
                _state.Set(GameState.Playing);
        }
    }
}
