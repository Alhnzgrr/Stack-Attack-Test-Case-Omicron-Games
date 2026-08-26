using System;

namespace StackAttack.Core
{
    public enum GroupLayout
    {
        Single,
        Row,
        Ring,
        Cluster
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

        // Set on an entry that places a boss instead of a stack group. The stack
        // fields above then describe the ring of guards standing around it.
        public BossConfig boss;
    }
}
