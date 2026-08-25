using UnityEngine;

namespace StackAttack.Core
{
    public interface IShardBurst
    {
        void Burst(Vector3 position, Color color, int count);
    }
}
