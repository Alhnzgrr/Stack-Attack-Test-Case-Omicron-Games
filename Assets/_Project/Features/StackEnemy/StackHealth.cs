using UnityEngine;

namespace StackAttack.StackEnemy
{
    public class StackHealth
    {
        private readonly float _hitsPerPlate;
        private readonly int _maxPlates;

        public StackHealth(float maxHp, float hitsPerPlate, int maxPlates)
        {
            _hitsPerPlate = hitsPerPlate;
            _maxPlates = maxPlates;
            CurrentHp = maxHp;
        }

        public float CurrentHp { get; private set; }

        public bool IsDead => CurrentHp <= 0f;

        public int VisiblePlates => Mathf.Clamp(Mathf.CeilToInt(CurrentHp / _hitsPerPlate), 0, _maxPlates);

        public void TakeDamage(float amount)
        {
            CurrentHp = Mathf.Max(CurrentHp - amount, 0f);
        }
    }
}
