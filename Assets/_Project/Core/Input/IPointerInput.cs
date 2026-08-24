namespace StackAttack.Core
{
    public interface IPointerInput
    {
        float DeltaPixels { get; }
        bool IsPressed { get; }

        bool PressedThisFrame { get; }
    }
}
