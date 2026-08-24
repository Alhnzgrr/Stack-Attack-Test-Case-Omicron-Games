using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    public class PlayerMovementBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform body;

        private IDragInput _dragInput;
        private PlayerMovement _movement;

        [Inject]
        public void Construct(IDragInput dragInput, PlayerConfig config, PlayfieldConfig playfield)
        {
            _dragInput = dragInput;
            _movement = new PlayerMovement(config, playfield.HalfWidth, transform.position.x);
        }

        private void Update()
        {
            _movement.Tick(_dragInput.DeltaPixels / Screen.width, Time.deltaTime);

            var position = transform.position;
            position.x = _movement.CurrentX;
            transform.position = position;

            body.localRotation = Quaternion.Euler(0f, 0f, _movement.TiltAngle);
        }
    }
}
