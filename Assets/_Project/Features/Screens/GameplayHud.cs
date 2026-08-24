using StackAttack.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace StackAttack.Screens
{
    public class GameplayHud : MonoBehaviour
    {
        [SerializeField] private Image progressFill;
        [SerializeField] private TextMeshProUGUI levelLabel;
        [SerializeField] private GameObject[] hearts;

        private LevelProgress _progress;
        private PlayerHealth _health;

        [Inject]
        public void Construct(LevelProgress progress, PlayerHealth health)
        {
            _progress = progress;
            _health = health;
        }

        private void OnEnable()
        {
            levelLabel.SetText("Level {0:0}", _progress.LevelIndex + 1);
        }

        private void Update()
        {
            progressFill.fillAmount = _progress.Normalized;

            for (int i = 0; i < hearts.Length; i++)
                hearts[i].SetActive(i < _health.Current);
        }
    }
}
