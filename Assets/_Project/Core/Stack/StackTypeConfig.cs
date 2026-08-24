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

        public int HitsPerPlate => hitsPerPlate;
        public Color PlateColor => plateColor;
        public Vector2 PlateSize => plateSize;
        public float PlateStep => plateStep;
        public int ContactDamage => contactDamage;
    }
}
