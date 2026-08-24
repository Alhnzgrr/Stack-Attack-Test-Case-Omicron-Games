using UnityEngine;

namespace StackAttack.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "StackAttack/Player/Player Config")]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField] private float dragSensitivity = 1f;
        [SerializeField] private float followSmoothTime = 0.06f;
        [SerializeField] private float maxTiltAngle = 15f;
        [SerializeField] private float tiltSpeedReference = 12f;
        [SerializeField] private float tiltSmoothTime = 0.08f;

        public float DragSensitivity => dragSensitivity;
        public float FollowSmoothTime => followSmoothTime;
        public float MaxTiltAngle => maxTiltAngle;
        public float TiltSpeedReference => tiltSpeedReference;
        public float TiltSmoothTime => tiltSmoothTime;
    }
}
