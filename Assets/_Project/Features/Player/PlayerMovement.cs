using UnityEngine;

namespace StackAttack.Player
{
    public class PlayerMovement
    {
        private readonly PlayerConfig _config;
        private readonly float _halfWidth;

        private float _targetX;
        private float _currentX;
        private float _velocity;

        private float _tiltAngle;
        private float _tiltVelocity;

        public PlayerMovement(PlayerConfig config, float halfWidth, float startX)
        {
            _config = config;
            _halfWidth = halfWidth;
            _targetX = startX;
            _currentX = startX;
        }

        public float CurrentX => _currentX;
        public float TiltAngle => _tiltAngle;

        // normalizedDragDelta is the drag of this frame as a fraction of screen width,
        // so the same swipe covers the same playfield distance on any resolution.
        public void Tick(float normalizedDragDelta, float deltaTime)
        {
            var worldDelta = normalizedDragDelta * _halfWidth * 2f * _config.DragSensitivity;

            _targetX = Mathf.Clamp(_targetX + worldDelta, -_halfWidth, _halfWidth);
            _currentX = Mathf.SmoothDamp(
                _currentX,
                _targetX,
                ref _velocity,
                _config.FollowSmoothTime,
                Mathf.Infinity,
                deltaTime);

            UpdateTilt(deltaTime);
        }

        // Leaning is driven by the movement velocity, so it eases back to zero on its
        // own as soon as the player stops chasing the finger.
        private void UpdateTilt(float deltaTime)
        {
            var normalizedSpeed = Mathf.Clamp(_velocity / _config.TiltSpeedReference, -1f, 1f);
            var desiredTilt = -normalizedSpeed * _config.MaxTiltAngle;

            _tiltAngle = Mathf.SmoothDamp(
                _tiltAngle,
                desiredTilt,
                ref _tiltVelocity,
                _config.TiltSmoothTime,
                Mathf.Infinity,
                deltaTime);
        }
    }
}
