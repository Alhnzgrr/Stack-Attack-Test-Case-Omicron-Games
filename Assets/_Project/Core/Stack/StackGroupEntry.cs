using System;

namespace StackAttack.Core
{
    public enum GroupLayout
    {
        Single,
        Row,
        Ring
    }

    public enum GroupMotion
    {
        Static,
        Horizontal,
        Orbit
    }

    [Serializable]
    public class StackGroupEntry
    {
        public float distance;
        public float xPosition;
        public StackTypeConfig stackType;
        public int hp;
        public GroupLayout layout;
        public int count;
        public float spacing;
        public float radius;
        public GroupMotion motion;
        public float motionSpeed;
    }
}
