using StackAttack.Core;
using UnityEngine;

namespace StackAttack.StackEnemy
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BossBehaviour : MonoBehaviour, IDamageable
    {
        [SerializeField] private SpriteRenderer body;
        [SerializeField] private Transform healthFill;
        [SerializeField] private Transform guards;

        [SerializeField] private float hitFlashDuration = 0.1f;
        [SerializeField] private float hitFlashStrength = 0.7f;
        [SerializeField] private float hitPunchScale = 0.05f;

        private StackBehaviour[] _members;
        private BoxCollider2D[] _guardBounds;
        private BoxCollider2D _collider;
        private HitFeedback _feedback;

        private BossConfig _config;
        private IStackSpawner _spawner;
        private IScoreSink _score;
        private IShardBurst _shards;

        private StackGroupEntry _shot;
        private Vector3[] _baseOffsets;
        private Vector3 _bodyScale;
        private float _guardSpin;
        private float _spinAngle;
        private float _hp;
        private float _unscored;
        private float _swayTime;
        private float _swayRange;
        private float _swayCentre;
        private float _shotTimer;
        private float _muzzleDrop;
        private bool _arrived;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _members = guards.GetComponentsInChildren<StackBehaviour>(true);
            _baseOffsets = new Vector3[_members.Length];
            _guardBounds = new BoxCollider2D[_members.Length];

            for (int i = 0; i < _members.Length; i++)
                _guardBounds[i] = _members[i].GetComponent<BoxCollider2D>();
        }

        public void Setup(StackGroupEntry entry, PlayfieldConfig playfield, IStackSpawner spawner, IScoreSink score, IShardBurst shards)
        {
            _config = entry.boss;
            _spawner = spawner;
            _score = score;
            _shards = shards;

            _hp = _config.MaxHp;
            _unscored = 0f;
            _swayTime = 0f;
            _arrived = false;
            _feedback = new HitFeedback(hitFlashDuration, hitFlashStrength, hitPunchScale);

            Dress(_config);
            Surround(entry, playfield);
            LoadShot(_config);

            transform.position = new Vector3(_swayCentre, playfield.SpawnY, 0f);

            ApplyHealth();
            ApplyFeedback();
        }

        private void Dress(BossConfig config)
        {
            _bodyScale = new Vector3(config.BodySize, config.BodySize, 1f);

            body.color = config.BodyColor;
            body.transform.localScale = _bodyScale;

            // The sprite is a unit square at its import scale, so the body size is
            // what turns it into a world footprint the shots have to get past.
            Vector2 footprint = body.sprite.bounds.size * config.BodySize;

            _collider.size = footprint;
            _collider.offset = Vector2.zero;
            _muzzleDrop = footprint.y * 0.5f;
        }

        // The ring is read off the entry, which is where the level places and sizes
        // it, and the entry's motion speed is how fast the guards go around.
        private void Surround(StackGroupEntry entry, PlayfieldConfig playfield)
        {
            StackGroupEntry ring = StackGroupGeometry.GuardRing(entry);

            ring.count = Mathf.Clamp(ring.count, 0, _members.Length);

            _guardSpin = entry.motionSpeed;
            _spinAngle = 0f;

            for (int i = 0; i < _members.Length; i++)
            {
                bool used = i < ring.count;
                _members[i].gameObject.SetActive(used);
                _baseOffsets[i] = Vector3.zero;

                if (!used)
                    continue;

                _members[i].Setup(entry.stackType, entry.hp, _score, _shards);

                Vector2 offset = StackGroupGeometry.MemberOffset(ring, i, ring.count);
                _baseOffsets[i] = new Vector3(offset.x, offset.y, 0f);
            }

            PlaceGuards();

            // It stands where the level put it, and sways as far either side as the
            // playfield leaves once the ring is accounted for.
            float limit = Mathf.Max(playfield.HalfWidth - StackGroupGeometry.HalfWidth(ring), 0f);

            _swayCentre = Mathf.Clamp(entry.xPosition, -limit, limit);
            _swayRange = Mathf.Min(_config.SwayRange, limit - Mathf.Abs(_swayCentre));
        }

        private void LoadShot(BossConfig config)
        {
            _shot = new StackGroupEntry
            {
                stackType = config.ShotType,
                hp = config.ShotHp,
                layout = config.ShotCount > 1 ? GroupLayout.Row : GroupLayout.Single,
                count = config.ShotCount,
                spacing = config.ShotSpacing,
                motion = GroupMotion.Static
            };
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            if (deltaTime <= 0f)
                return;

            if (_arrived)
            {
                Sway(deltaTime);
                Shoot(deltaTime);
            }
            else
            {
                Arrive(deltaTime);
            }

            _spinAngle += _guardSpin * deltaTime;
            PlaceGuards();

            if (!_feedback.IsActive)
                return;

            _feedback.Tick(deltaTime);
            ApplyFeedback();
        }

        private void Arrive(float deltaTime)
        {
            Vector3 position = transform.position;
            position.y = Mathf.MoveTowards(position.y, _config.HoldY, _config.EntrySpeed * deltaTime);
            transform.position = position;

            if (position.y > _config.HoldY)
                return;

            _arrived = true;
            _shotTimer = _config.FirstShotDelay;
        }

        // The guards turn around the body instead of the body turning, so the boss
        // keeps facing the player from the exact middle of its own ring.
        //
        // A guard sits on the ring by the middle of its silhouette rather than by
        // its feet, and that middle drops every time a plate breaks, so it is read
        // off the guard's own collider each frame rather than guessed once from the
        // plate count. This is what keeps the body dead centre of the ring.
        private void PlaceGuards()
        {
            Quaternion rotation = Quaternion.Euler(0f, 0f, _spinAngle);

            for (int i = 0; i < _members.Length; i++)
            {
                if (!_members[i].gameObject.activeSelf)
                    continue;

                Vector3 point = rotation * _baseOffsets[i];

                _members[i].transform.localPosition = new Vector3(point.x, point.y - _guardBounds[i].offset.y, 0f);
            }
        }

        private void Sway(float deltaTime)
        {
            _swayTime += deltaTime * _config.SwaySpeed;
            transform.position = new Vector3(_swayCentre + Mathf.Sin(_swayTime) * _swayRange, _config.HoldY, 0f);
        }

        private void Shoot(float deltaTime)
        {
            _shotTimer -= deltaTime;

            if (_shotTimer > 0f)
                return;

            _shotTimer = _config.ShotInterval;

            // Thrown from under the body so it slides out of the boss rather than
            // appearing in front of it, and straight down from wherever it stands.
            _shot.xPosition = transform.position.x;
            _spawner.SpawnAt(_shot, _config.ShotSpeed, transform.position.y - _muzzleDrop);
        }

        public void TakeDamage(float amount)
        {
            _hp = Mathf.Max(_hp - amount, 0f);

            // A stack pays out a whole plate at a time. The boss has no plates to
            // break, so it pays a point per hit point instead and carries the
            // fraction over rather than rounding every shot in the players favour.
            _unscored += amount;
            int points = Mathf.FloorToInt(_unscored);

            if (points > 0)
            {
                _score.AddPoints(points);
                _unscored -= points;
            }

            ApplyHealth();

            if (_hp <= 0f)
            {
                Die();
                return;
            }

            _feedback.Restart();
            ApplyFeedback();
        }

        private void Die()
        {
            _shards.Burst(body.transform.position, _config.BodyColor, _config.DeathShards);
            Destroy(gameObject);
        }

        private void ApplyHealth()
        {
            healthFill.localScale = new Vector3(_hp / _config.MaxHp, 1f, 1f);
        }

        private void ApplyFeedback()
        {
            body.color = Color.Lerp(_config.BodyColor, Color.white, _feedback.FlashAmount);
            body.transform.localScale = _bodyScale * _feedback.ScaleMultiplier;
        }
    }
}
