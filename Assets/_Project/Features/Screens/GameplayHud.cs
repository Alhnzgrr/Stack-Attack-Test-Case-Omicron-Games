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

        private int _shownLevelIndex;

        [Inject]
        public void Construct(LevelProgress progress, PlayerHealth health, RunScore score)
        {
            _progress = progress;
            _health = health;
            _score = score;
        }

        // LevelRunner refreshes the index from the same state change that opens this
        // panel, and the order the two subscribers run in is not defined. Reading the
        // value every frame instead of once on enable keeps the label correct either
        // way.
        private void OnEnable()
        {
            _shownLevelIndex = -1;
        }

        private void Update()
        {
            if (_shownLevelIndex != _progress.LevelIndex)
            {
                _shownLevelIndex = _progress.LevelIndex;
                levelLabel.SetText("Level {0:0}", _shownLevelIndex + 1);
            }

            levelFill.fillAmount = _progress.Normalized;
            upgradeFill.fillAmount = _score.Normalized;

            for (int i = 0; i < hearts.Length; i++)
                hearts[i].SetActive(i < _health.Current);
        }
    }
}
