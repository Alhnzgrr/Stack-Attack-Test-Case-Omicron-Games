using UnityEngine.InputSystem;

namespace StackAttack.Core
{
    public class PointerInput : IPointerInput
    {
        public float DeltaPixels => IsPressed ? Pointer.current.delta.ReadValue().x : 0f;

        public bool IsPressed
        {
            get
            {
                Pointer pointer = Pointer.current;
                return pointer != null && pointer.press.isPressed;
            }
        }

        public bool PressedThisFrame
        {
            get
            {
                Pointer pointer = Pointer.current;
                return pointer != null && pointer.press.wasPressedThisFrame;
            }
        }
    }
}
