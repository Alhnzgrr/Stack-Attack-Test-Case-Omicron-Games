using StackAttack.Core;
using UnityEngine;

namespace StackAttack.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 14f;
        [SerializeField] private float maxDistance = 20f;

        private ThrownPool<Projectile> _pool;
        private float _damage;
        private float _travelled;
        private int _hitsLeft;

        public void Launch(float damage, float size, int pierce, ThrownPool<Projectile> pool)
        {
            _pool = pool;
            _damage = damage;
            _travelled = 0f;
            _hitsLeft = pierce + 1;
            transform.localScale = new Vector3(size, size, 1f);
        }

        private void Update()
        {
            float step = speed * Time.deltaTime;
            transform.Translate(Vector3.up * step, Space.World);

            _travelled += step;
            if (_travelled >= maxDistance)
                _pool.Release(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out IDamageable damageable))
                return;

            damageable.TakeDamage(_damage);

            _hitsLeft--;
            if (_hitsLeft <= 0)
                _pool.Release(this);
        }
    }
}
