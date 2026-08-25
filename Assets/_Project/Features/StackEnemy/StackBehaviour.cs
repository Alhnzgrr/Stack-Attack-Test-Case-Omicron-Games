using StackAttack.Core;
using TMPro;
using UnityEngine;

namespace StackAttack.StackEnemy
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class StackBehaviour : MonoBehaviour, IDamageable, IHazard
    {
        [SerializeField] private StackTypeConfig type;
        [SerializeField] private int maxHp = 12;

        private SpriteRenderer[] _plates;
        private TextMeshPro _label;
        private BoxCollider2D _collider;

        private StackTypeConfig _type;
        private StackHealth _health;
        private StackHitFeedback _feedback;
        private Vector3 _baseScale;
        private Vector2 _plateSpriteSize;

        private void Awake()
        {
            _plates = GetComponentsInChildren<SpriteRenderer>(true);
            _label = GetComponentInChildren<TextMeshPro>(true);
            _collider = GetComponent<BoxCollider2D>();
            _baseScale = transform.localScale;
            // PlateSize is a localScale multiplier, not a world size, so the footprint
            // of one plate can only come from the sprite itself.
            _plateSpriteSize = _plates[0].sprite.bounds.size;
        }

        // Hand-placed stacks initialise themselves from the inspector values. Once the
        // level spawner exists it calls Setup first and this fallback does nothing.
        private void Start()
        {
            if (_health == null)
                Setup(type, maxHp);
        }

        public void Setup(StackTypeConfig stackType, int hp)
        {
            _type = stackType;
            _health = new StackHealth(hp, stackType.HitsPerPlate, _plates.Length);
            _feedback = new StackHitFeedback(
                stackType.HitFlashDuration,
                stackType.HitFlashStrength,
                stackType.HitPunchScale);

            transform.localScale = _baseScale;

            for (int i = 0; i < _plates.Length; i++)
            {
                SpriteRenderer plate = _plates[i];

                plate.color = stackType.PlateColor;
                plate.sortingOrder = i;
                plate.transform.localPosition = new Vector3(0f, i * stackType.PlateStep, 0f);
                plate.transform.localScale = new Vector3(stackType.PlateSize.x, stackType.PlateSize.y, 1f);
            }

            Apply();
        }

        public int ContactDamage => _type.ContactDamage;

        public void TakeDamage(float amount)
        {
            _health.TakeDamage(amount);
            Apply();

            if (_health.IsDead)
                return;

            // Restarting here rather than on the next frame keeps the flash on the
            // same frame as the hit, which is what makes the shot feel connected.
            _feedback.Restart();
            ApplyFeedback();
        }

        private void Update()
        {
            if (!_feedback.IsActive)
                return;

            _feedback.Tick(Time.deltaTime);
            ApplyFeedback();
        }

        // The whole stack reacts, not just the plate that was hit, so the pulse is
        // written across every plate and the punch onto the stack root.
        private void ApplyFeedback()
        {
            Color color = Color.Lerp(_type.PlateColor, Color.white, _feedback.FlashAmount);

            for (int i = 0; i < _plates.Length; i++)
                _plates[i].color = color;

            transform.localScale = _baseScale * _feedback.ScaleMultiplier;
        }

        private void Apply()
        {
            int visiblePlates = _health.VisiblePlates;

            for (int i = 0; i < _plates.Length; i++)
                _plates[i].gameObject.SetActive(i < visiblePlates);

            if (_health.IsDead)
            {
                gameObject.SetActive(false);
                return;
            }

            float topPlateY = (visiblePlates - 1) * _type.PlateStep;
            float plateWidth = _plateSpriteSize.x * _type.PlateSize.x;
            float plateHeight = _plateSpriteSize.y * _type.PlateSize.y;

            // The silhouette runs from the bottom of the lowest plate to the top of the
            // highest, which is one plate taller than the stepping alone suggests.
            _collider.size = new Vector2(plateWidth, topPlateY + plateHeight);
            _collider.offset = new Vector2(0f, topPlateY * 0.5f);

            _label.transform.localPosition = new Vector3(0f, topPlateY, -0.01f);
            _label.SetText("{0:0}", Mathf.CeilToInt(_health.CurrentHp));
        }
    }
}
