using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Screens
{
    public class ScreenRouter : MonoBehaviour
    {
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject endPanel;

        private GameStateMachine _state;

        [Inject]
        public void Construct(GameStateMachine state)
        {
            _state = state;
        }

        private void Start()
        {
            _state.Changed += Apply;
            Apply(_state.Current);
        }

        private void OnDestroy()
        {
            _state.Changed -= Apply;
        }

        private void Apply(GameState state)
        {
            startPanel.SetActive(state == GameState.Start);
            hudPanel.SetActive(state == GameState.Playing);
            endPanel.SetActive(state == GameState.Won || state == GameState.Lost);
        }
    }
}
