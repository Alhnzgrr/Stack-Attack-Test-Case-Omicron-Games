namespace StackAttack.Core
{
    public interface IStackSpawner
    {
        bool HasActiveGroups { get; }

        void Spawn(StackGroupEntry entry, float descentSpeed);
        void Tick();
        void Clear();
    }
}
