using StackAttack.Core;
using TMPro;
using UnityEngine;
using VContainer;

namespace StackAttack.Screens
{
    public class GameplayHud : MonoBehaviour
    {
        [SerializeField] private RectTransform levelFill;
        [SerializeField] private TextMeshProUGUI levelLabel;
        [SerializeField] private GameObject[] hearts;

        private LevelProgress _progress;
        private PlayerHealth _health;

        private int _shownLevelIndex;
        private Vector2 _levelFillFullSize;

        [Inject]
        public void Construct(LevelProgress progress, PlayerHealth health)
        {
            _progress = progress;
            _health = health;
        }

        private void Awake()
        {
            // The fill is authored at its full extent, so the size it has in the editor
            // is the hundred percent mark everything else is scaled against.
            _levelFillFullSize = levelFill.rect.size;
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

            // Driving the size rather than fillAmount keeps the sprite sliced, so the
            // rounded cap survives at every fill level.
            levelFill.sizeDelta = new Vector2(
                _levelFillFullSize.x,
                _levelFillFullSize.y * _progress.Normalized);

            for (int i = 0; i < hearts.Length; i++)
                hearts[i].SetActive(i < _health.Current);
        }
    }
}
