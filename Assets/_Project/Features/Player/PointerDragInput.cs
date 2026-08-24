using UnityEngine.InputSystem;

namespace StackAttack.Player
{
    public class PointerDragInput : IDragInput
    {
        public float DeltaPixels
        {
            get
            {
                var pointer = Pointer.current;
                if (pointer == null || !pointer.press.isPressed)
                    return 0f;

                return pointer.delta.ReadValue().x;
            }
        }
    }
}
