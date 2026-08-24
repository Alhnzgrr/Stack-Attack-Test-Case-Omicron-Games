using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Player
{
    public class PlayerMovementBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform body;

        private IPointerInput _pointerInput;
        private PlayerMovement _movement;

        [Inject]
        public void Construct(IPointerInput pointerInput, PlayerConfig config, PlayfieldConfig playfield)
        {
            _pointerInput = pointerInput;
            _movement = new PlayerMovement(config, playfield.HalfWidth, transform.position.x);
        }

        private void Update()
        {
            _movement.Tick(_pointerInput.DeltaPixels / Screen.width, Time.deltaTime);

            Vector3 position = transform.position;
            position.x = _movement.CurrentX;
            transform.position = position;

            body.localRotation = Quaternion.Euler(0f, 0f, _movement.TiltAngle);
        }
    }
}
