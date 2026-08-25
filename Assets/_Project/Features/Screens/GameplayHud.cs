using StackAttack.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace StackAttack.Screens
{
    public class GameplayHud : MonoBehaviour
    {
        [SerializeField] private Image levelFill;
        [SerializeField] private Image upgradeFill;
        [SerializeField] private TextMeshProUGUI levelLabel;
        [SerializeField] private GameObject[] hearts;

        private LevelProgress _progress;
        private PlayerHealth _health;
        private RunScore _score;

        [Inject]
        public void Construct(LevelProgress progress, PlayerHealth health, RunScore score)
        {
            _progress = progress;
            _health = health;
            _score = score;
        }

        // Subscribing in Start rather than OnEnable because the services arrive
        // through injection at the scope's Awake, which is not ordered against this
        // component's own Awake. The subscriptions then outlive the panel being
        // closed between levels, so nothing is missed while it is hidden.
        private void Start()
        {
            _health.Changed += ApplyHearts;
            _score.Changed += ApplyUpgradeBar;
            _progress.LevelChanged += ApplyLevelLabel;

            ApplyHearts();
            ApplyUpgradeBar();
            ApplyLevelLabel();
        }

        private void OnDestroy()
        {
            _health.Changed -= ApplyHearts;
            _score.Changed -= ApplyUpgradeBar;
            _progress.LevelChanged -= ApplyLevelLabel;
        }

        private void Update()
        {
            // Travelled advances every frame, so this one genuinely belongs here.
            levelFill.fillAmount = _progress.Normalized;
        }

        private void ApplyHearts()
        {
            for (int i = 0; i < hearts.Length; i++)
                hearts[i].SetActive(i < _health.Current);
        }

        private void ApplyUpgradeBar()
        {
            upgradeFill.fillAmount = _score.Normalized;
        }

        private void ApplyLevelLabel()
        {
            levelLabel.SetText("Level {0:0}", _progress.LevelIndex + 1);
        }
    }
}
