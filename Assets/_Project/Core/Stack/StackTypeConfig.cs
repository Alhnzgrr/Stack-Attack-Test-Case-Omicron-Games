using UnityEngine;

namespace StackAttack.Core
{
    [CreateAssetMenu(fileName = "StackType", menuName = "StackAttack/Stack/Stack Type")]
    public class StackTypeConfig : ScriptableObject
    {
        [SerializeField] private int hitsPerPlate = 2;
        [SerializeField] private Color plateColor = new Color(0.6f, 0.3f, 0.8f, 1f);
        [SerializeField] private Vector2 plateSize = new Vector2(1f, 1f);
        [SerializeField] private float plateStep = 0.1f;
        [SerializeField] private int contactDamage = 1;
        [SerializeField] private int maxPlates = 15;

        [SerializeField] private float hitFlashDuration = 0.12f;
        [SerializeField] private float hitFlashStrength = 0.8f;
        [SerializeField] private float hitPunchScale = 0.08f;
        [SerializeField] private int shardsPerPlate = 7;

        public int HitsPerPlate => hitsPerPlate;
        public Color PlateColor => plateColor;
        public Vector2 PlateSize => plateSize;
        public float PlateStep => plateStep;
        public int ContactDamage => contactDamage;
        public int MaxPlates => maxPlates;
        public float HitFlashDuration => hitFlashDuration;
        public float HitFlashStrength => hitFlashStrength;
        public float HitPunchScale => hitPunchScale;
        public int ShardsPerPlate => shardsPerPlate;
    }
}
