using UnityEngine;

namespace StackAttack.Core
{
    [CreateAssetMenu(fileName = "BossConfig", menuName = "StackAttack/Boss/Boss Config")]
    public class BossConfig : ScriptableObject
    {
        [SerializeField] private float maxHp = 350f;
        [SerializeField] private Color bodyColor = new Color(0.35f, 0.85f, 0.4f, 1f);
        [SerializeField] private float bodySize = 2.2f;

        [SerializeField] private float holdY = 1.6f;
        [SerializeField] private float entrySpeed = 2.5f;
        [SerializeField] private float swayRange = 1.2f;
        [SerializeField] private float swaySpeed = 0.8f;

        [SerializeField] private StackTypeConfig shotType;
        [SerializeField] private int shotHp = 10;
        [SerializeField] private int shotCount = 1;
        [SerializeField] private float shotSpacing = 1.1f;
        [SerializeField] private float shotSpeed = 2.6f;
        [SerializeField] private float shotInterval = 2.4f;
        [SerializeField] private float firstShotDelay = 1.5f;

        [SerializeField] private int deathShards = 60;

        public float MaxHp => maxHp;
        public Color BodyColor => bodyColor;
        public float BodySize => bodySize;

        public float HoldY => holdY;
        public float EntrySpeed => entrySpeed;
        public float SwayRange => swayRange;
        public float SwaySpeed => swaySpeed;

        public StackTypeConfig ShotType => shotType;
        public int ShotHp => shotHp;
        public int ShotCount => shotCount;
        public float ShotSpacing => shotSpacing;
        public float ShotSpeed => shotSpeed;
        public float ShotInterval => shotInterval;
        public float FirstShotDelay => firstShotDelay;

        public int DeathShards => deathShards;
    }
}
