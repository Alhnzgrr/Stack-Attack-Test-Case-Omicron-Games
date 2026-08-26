using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerMovementBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform body;

        private IPointerInput _pointerInput;
        private GameStateMachine _state;
        private PlayerConfig _config;
        private PlayfieldConfig _playfield;
        private BoxCollider2D _collider;

        private float _limit;
        private PlayerMovement _movement;

        [Inject]
        public void Construct(IPointerInput pointerInput, PlayerConfig config, PlayfieldConfig playfield, GameStateMachine state)
        {
            _pointerInput = pointerInput;
            _state = state;
            _config = config;
            _playfield = playfield;
        }

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
        }

        // Injection and Awake race each other, so the limit is measured here where
        // both the container and the collider are known to be ready.
        private void Start()
        {
            // The edge of the playfield pulled in by the player's own body, the same
            // way a stack group keeps its whole silhouette inside the field.
            _limit = Mathf.Max(_playfield.HalfWidth - _collider.bounds.extents.x, 0f);
            _movement = new PlayerMovement(_config, _limit, transform.position.x);

            _state.Changed += OnStateChanged;
        }

        private void OnDestroy()
        {
            _state.Changed -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Playing)
                _movement = new PlayerMovement(_config, _limit, 0f);
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
