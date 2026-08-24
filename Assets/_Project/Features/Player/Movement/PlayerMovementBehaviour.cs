using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    public class PlayerMovementBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform body;

        private IPointerInput _pointerInput;
        private GameStateMachine _state;
        private PlayerConfig _config;
        private float _halfWidth;
        private PlayerMovement _movement;

        [Inject]
        public void Construct(IPointerInput pointerInput, PlayerConfig config, PlayfieldConfig playfield, GameStateMachine state)
        {
            _pointerInput = pointerInput;
            _state = state;
            _config = config;
            _halfWidth = playfield.HalfWidth;
            _movement = new PlayerMovement(config, playfield.HalfWidth, transform.position.x);
        }

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
            if (state == GameState.Playing)
                _movement = new PlayerMovement(_config, _halfWidth, 0f);
        }

        private void Update()
        {
            if (_state.Current != GameState.Playing)
                return;

            _movement.Tick(_pointerInput.DeltaPixels / Screen.width, Time.deltaTime);

            Vector3 position = transform.position;
            position.x = _movement.CurrentX;
            transform.position = position;

            body.localRotation = Quaternion.Euler(0f, 0f, _movement.TiltAngle);
        }
    }
}
