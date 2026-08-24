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

        private float _damage;
        private float _travelled;

        public void Launch(float damage, float size)
        {
            _damage = damage;
            _travelled = 0f;
            transform.localScale = new Vector3(size, size, 1f);
        }

        private void Update()
        {
            float step = speed * Time.deltaTime;
            transform.Translate(Vector3.up * step, Space.World);

            _travelled += step;
            if (_travelled >= maxDistance)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out IDamageable damageable))
                return;

            damageable.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
