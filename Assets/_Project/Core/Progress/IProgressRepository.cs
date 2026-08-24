namespace StackAttack.Core
{
    public interface IProgressRepository
    {
        int LastLevelIndex { get; }

        void SaveLastLevelIndex(int index);
    }
}
