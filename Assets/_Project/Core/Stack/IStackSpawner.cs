namespace StackAttack.Core
{
    public interface IStackSpawner
    {
        void Spawn(StackGroupEntry entry, float descentSpeed);
        void Tick();
        void Clear();
    }
}
