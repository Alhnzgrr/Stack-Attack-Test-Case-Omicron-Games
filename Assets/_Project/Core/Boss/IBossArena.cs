namespace StackAttack.Core
{
    public interface IBossArena
    {
        bool IsActive { get; }

        void Begin(StackGroupEntry entry);
        void Clear();
    }
}
