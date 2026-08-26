using StackAttack.Core;
using UnityEngine;

namespace StackAttack.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private float speed = 7f;
        [SerializeField] private float turnRate = 300f;
        [SerializeField] private float searchRadius = 9f;
        [SerializeField] private float blastRadius = 1.6f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private int blastShards = 20;
        [SerializeField] private Color blastColor = new Color(1f, 0.6f, 0.15f, 1f);

        private IShardBurst _shards;
        private Transform _target;
        private Vector3 _heading;
        private float _damage;
        private float _elapsed;

        public void Launch(float damage, IShardBurst shards)
        {
            _damage = damage;
            _shards = shards;
            _elapsed = 0f;
            _heading = Vector3.up;
            _target = Nearest(searchRadius);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            _elapsed += deltaTime;

            Steer(deltaTime);

            transform.position += _heading * (speed * deltaTime);
            transform.up = _heading;

            if (_elapsed >= lifetime)
                Explode();
        }

        // The target is found once and then only followed. Stacks switch themselves
        // off as they die, and losing one mid flight means carrying on along the last
        // heading rather than stopping dead in the air.
        private void Steer(float deltaTime)
        {
            if (_target == null || !_target.gameObject.activeInHierarchy)
                return;

            Vector3 wanted = (_target.position - transform.position).normalized;

            _heading = Vector3.RotateTowards(_heading, wanted, turnRate * Mathf.Deg2Rad * deltaTime, 0f);
        }

        private Transform Nearest(float range)
        {
            Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, range);
            Transform nearest = null;
            float best = float.MaxValue;

            for (int i = 0; i < found.Length; i++)
            {
                if (!found[i].TryGetComponent(out IDamageable damageable))
                    continue;

                float distance = (found[i].transform.position - transform.position).sqrMagnitude;

                if (distance >= best)
                    continue;

                best = distance;
                nearest = found[i].transform;
            }

            return nearest;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
                Explode();
        }

        // Everything inside the blast takes the full hit. A falloff would make the
        // upgrade read as weaker than it is on the stacks the player aimed at.
        private void Explode()
        {
            Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, blastRadius);

            for (int i = 0; i < found.Length; i++)
            {
                if (found[i].TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(_damage);
            }

            _shards.Burst(transform.position, blastColor, blastShards);
            Destroy(gameObject);
        }
    }
}
