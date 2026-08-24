using StackAttack.Core;
using UnityEngine;

namespace StackAttack.StackEnemy
{
    public class StackGroup : MonoBehaviour
    {
        private StackBehaviour[] _members;
        private Vector3[] _baseOffsets;

        private GroupMotion _motion;
        private float _motionSpeed;
        private float _descentSpeed;

        private float _horizontalLimit;
        private int _horizontalDirection;
        private float _orbitAngle;

        private void Awake()
        {
            _members = GetComponentsInChildren<StackBehaviour>(true);
            _baseOffsets = new Vector3[_members.Length];
        }

        public void Setup(StackGroupEntry entry, float playfieldHalfWidth, float spawnY, float descentSpeed)
        {
            _motion = entry.motion;
            _motionSpeed = entry.motionSpeed;
            _descentSpeed = descentSpeed;
            _orbitAngle = 0f;
            _horizontalDirection = 1;

            int memberCount = Mathf.Clamp(MemberCount(entry), 1, _members.Length);
            BuildOffsets(entry, memberCount);

            for (int i = 0; i < _members.Length; i++)
            {
                bool used = i < memberCount;
                _members[i].gameObject.SetActive(used);

                if (!used)
                    continue;

                _members[i].Setup(entry.stackType, entry.hp);
                _members[i].transform.localPosition = _baseOffsets[i];
            }

            float stackWidth = entry.stackType.PlateSize.x;
            _horizontalLimit = Mathf.Max(playfieldHalfWidth - HalfWidthOf(entry, memberCount, stackWidth), 0f);

            transform.position = new Vector3(Mathf.Clamp(entry.xPosition, -_horizontalLimit, _horizontalLimit), spawnY, 0f);
        }

        public bool HasPassed(float despawnY)
        {
            return transform.position.y < despawnY;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            if (deltaTime <= 0f)
                return;

            transform.position += Vector3.down * (_descentSpeed * deltaTime);

            if (_motion == GroupMotion.Horizontal)
                MoveHorizontally(deltaTime);
            else if (_motion == GroupMotion.Orbit)
                Orbit(deltaTime);
        }

        private void MoveHorizontally(float deltaTime)
        {
            Vector3 position = transform.position;
            position.x += _motionSpeed * _horizontalDirection * deltaTime;

            if (position.x >= _horizontalLimit)
            {
                position.x = _horizontalLimit;
                _horizontalDirection = -1;
            }
            else if (position.x <= -_horizontalLimit)
            {
                position.x = -_horizontalLimit;
                _horizontalDirection = 1;
            }

            transform.position = position;
        }

        private void Orbit(float deltaTime)
        {
            _orbitAngle += _motionSpeed * deltaTime;
            Quaternion rotation = Quaternion.Euler(0f, 0f, _orbitAngle);

            for (int i = 0; i < _members.Length; i++)
            {
                if (!_members[i].gameObject.activeSelf)
                    continue;

                _members[i].transform.localPosition = rotation * _baseOffsets[i];
            }
        }

        private void BuildOffsets(StackGroupEntry entry, int memberCount)
        {
            for (int i = 0; i < _baseOffsets.Length; i++)
                _baseOffsets[i] = Vector3.zero;

            if (entry.layout == GroupLayout.Row)
            {
                float start = -(memberCount - 1) * 0.5f * entry.spacing;

                for (int i = 0; i < memberCount; i++)
                    _baseOffsets[i] = new Vector3(start + i * entry.spacing, 0f, 0f);
            }
            else if (entry.layout == GroupLayout.Ring)
            {
                float step = 360f / memberCount;

                for (int i = 0; i < memberCount; i++)
                {
                    float radians = step * i * Mathf.Deg2Rad;
                    _baseOffsets[i] = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * entry.radius;
                }
            }
        }

        private static int MemberCount(StackGroupEntry entry)
        {
            return entry.layout == GroupLayout.Single ? 1 : entry.count;
        }

        private static float HalfWidthOf(StackGroupEntry entry, int memberCount, float stackWidth)
        {
            float half = stackWidth * 0.5f;

            if (entry.layout == GroupLayout.Row)
                return (memberCount - 1) * 0.5f * entry.spacing + half;

            if (entry.layout == GroupLayout.Ring)
                return entry.radius + half;

            return half;
        }
    }
}
