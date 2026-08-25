using StackAttack.Core;
using UnityEngine;

namespace StackAttack.StackEnemy
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ShardBurst : MonoBehaviour, IShardBurst
    {
        private ParticleSystem _particles;

        private void Awake()
        {
            _particles = GetComponent<ParticleSystem>();
        }

        // This lives outside the stack pool on purpose. A member switches itself off
        // the moment it dies and the group is released right after, either of which
        // would take its own particles down with it mid-flight.
        public void Burst(Vector3 position, Color color, int count)
        {
            ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();
            emit.position = position;
            emit.applyShapeToPosition = true;
            emit.startColor = color;

            _particles.Emit(emit, count);
        }
    }
}
