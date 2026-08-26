using System.Collections.Generic;
using StackAttack.Core;
using UnityEngine;

namespace StackAttack.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Boomerang : MonoBehaviour
    {
        [SerializeField] private float duration = 1.6f;
        [SerializeField] private float reach = 4.5f;
        [SerializeField] private float radius = 1.1f;
        [SerializeField] private float turns = 1.5f;
        [SerializeField] private float spin = 720f;

        private readonly HashSet<IDamageable> _hit = new HashSet<IDamageable>();

        private ThrownPool<Boomerang> _pool;
        private Transform _owner;
        private float _damage;
        private float _phase;
        private float _elapsed;
        private bool _returning;

        public void Launch(Transform owner, float damage, float phase, ThrownPool<Boomerang> pool)
        {
            _pool = pool;
            _owner = owner;
            _damage = damage;
            _phase = phase;
            _elapsed = 0f;
            _returning = false;
            _hit.Clear();

            Place(0f);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            _elapsed += deltaTime;

            float progress = Mathf.Clamp01(_elapsed / duration);

            // Halfway out is where it turns for home, and it is also where it forgets
            // what it already hit, so a stack takes one hit going and one coming back
            // rather than one for every frame the two overlap.
            if (!_returning && progress >= 0.5f)
            {
                _returning = true;
                _hit.Clear();
            }

            Place(progress);
            transform.Rotate(0f, 0f, spin * deltaTime);

            if (progress >= 1f)
                _pool.Release(this);
        }

        // Sine over half a period both carries it out and brings it home, and it lands
        // on exactly zero, so the throw ends on the player rather than near them. The
        // offset is measured from where the player is now, not from where they threw.
        private void Place(float progress)
        {
            float envelope = Mathf.Sin(progress * Mathf.PI);
            float angle = _phase + progress * turns * Mathf.PI * 2f;

            Vector3 offset = Vector3.up * (reach * envelope)
                + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * (radius * envelope);

            transform.position = _owner.position + offset;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out IDamageable damageable))
                return;

            if (!_hit.Add(damageable))
                return;

            damageable.TakeDamage(_damage);
        }
    }
}
