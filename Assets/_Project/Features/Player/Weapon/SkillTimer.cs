namespace StackAttack.Player
{
    // Skills come round on their own clock rather than on the trigger, which is why
    // this is not AutoWeapon: nothing here asks whether a finger is down.
    public class SkillTimer
    {
        private readonly float _interval;

        private float _left;

        public SkillTimer(float interval)
        {
            _interval = interval;
            _left = interval;
        }

        public void Restart()
        {
            _left = _interval;
        }

        public bool Tick(float deltaTime)
        {
            if (_interval <= 0f)
                return false;

            _left -= deltaTime;

            if (_left > 0f)
                return false;

            // Assigning rather than adding drops the overshoot, so a long frame cannot
            // leave a debt that fires the skill twice in a row to pay it back.
            _left = _interval;
            return true;
        }
    }
}
