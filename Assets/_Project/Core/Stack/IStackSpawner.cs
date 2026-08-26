namespace StackAttack.Core
{
    public interface IStackSpawner
    {
        bool HasActiveGroups { get; }

        void Spawn(StackGroupEntry entry, float descentSpeed);
        void SpawnAt(StackGroupEntry entry, float descentSpeed, float spawnY);
        void Tick();
        void Clear();
    }
}
