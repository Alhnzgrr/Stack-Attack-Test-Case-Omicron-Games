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
            Cache();
        }

        private void Cache()
        {
            if (_members != null)
                return;

            _members = GetComponentsInChildren<StackBehaviour>(true);
            _baseOffsets = new Vector3[_members.Length];
        }

        public void Setup(StackGroupEntry entry, float playfieldHalfWidth, float spawnY, float descentSpeed, IScoreSink score, IShardBurst shards)
        {
            Cache();

            _motion = entry.motion;
            _motionSpeed = entry.motionSpeed;
            _descentSpeed = descentSpeed;
            _orbitAngle = 0f;
            _horizontalDirection = 1;

            int memberCount = Mathf.Clamp(StackGroupGeometry.MemberCount(entry), 1, _members.Length);
            BuildOffsets(entry, memberCount);

            for (int i = 0; i < _members.Length; i++)
            {
                bool used = i < memberCount;
                _members[i].gameObject.SetActive(used);

                if (!used)
                    continue;

                _members[i].Setup(entry.stackType, entry.hp, score, shards);
                _members[i].transform.localPosition = _baseOffsets[i];
            }

            _horizontalLimit = Mathf.Max(playfieldHalfWidth - StackGroupGeometry.HalfWidth(entry), 0f);

            transform.position = new Vector3(Mathf.Clamp(entry.xPosition, -_horizontalLimit, _horizontalLimit), spawnY, 0f);
        }

        public bool HasPassed(float despawnY)
        {
            return transform.position.y < despawnY;
        }

        // Members switch themselves off as they die. Waiting for the wreck to
        // scroll away would hold the group for another half minute at the slow
        // descent speeds the levels use.
        public bool IsCleared
        {
            get
            {
                for (int i = 0; i < _members.Length; i++)
                {
                    if (_members[i].gameObject.activeSelf)
                        return false;
                }

                return true;
            }
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

            for (int i = 0; i < memberCount; i++)
                _baseOffsets[i] = StackGroupGeometry.MemberOffset(entry, i, memberCount);
        }

    }
}
