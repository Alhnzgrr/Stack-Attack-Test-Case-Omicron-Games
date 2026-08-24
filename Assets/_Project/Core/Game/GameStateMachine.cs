using System;

namespace StackAttack.Core
{
    public enum GameState
    {
        Start,
        Playing,
        Won,
        Lost
    }

    public class GameStateMachine
    {
        public GameState Current { get; private set; } = GameState.Start;

        public event Action<GameState> Changed;

        public void Set(GameState state)
        {
            if (Current == state)
                return;

            Current = state;
            Changed?.Invoke(state);
        }
    }
}
