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

        private void Awake()
        {
            _plates = GetComponentsInChildren<SpriteRenderer>(true);
            _label = GetComponentInChildren<TextMeshPro>(true);
            _collider = GetComponent<BoxCollider2D>();
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

            _collider.size = new Vector2(_type.PlateSize.x, visiblePlates * _type.PlateStep);
            _collider.offset = new Vector2(0f, topPlateY * 0.5f);

            _label.transform.localPosition = new Vector3(0f, topPlateY, -0.01f);
            _label.SetText("{0:0}", Mathf.CeilToInt(_health.CurrentHp));
        }
    }
}
