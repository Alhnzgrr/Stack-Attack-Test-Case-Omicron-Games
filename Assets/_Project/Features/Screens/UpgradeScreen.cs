using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using StackAttack.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace StackAttack.Screens
{
    public class UpgradeScreen : MonoBehaviour
    {
        [Serializable]
        private class Card
        {
            public Button button;
            public CanvasGroup group;
            public Image frame;
            public Image icon;
            public TextMeshProUGUI category;
            public TextMeshProUGUI stat;
            public TextMeshProUGUI value;
        }

        [SerializeField] private GameObject panel;
        [SerializeField] private Card[] cards;

        [SerializeField] private float introDuration = 0.28f;
        [SerializeField] private float introStagger = 0.07f;
        [SerializeField] private float introPunch = 0.12f;

        private IUpgradeService _upgrades;
        private CancellationTokenSource _introCts;

        [Inject]
        public void Construct(IUpgradeService upgrades)
        {
            _upgrades = upgrades;
        }

        private void Awake()
        {
            for (int i = 0; i < cards.Length; i++)
            {
                int index = i;
                cards[i].button.onClick.AddListener(() => _upgrades.Choose(index));
            }

            panel.SetActive(false);
        }

        // Subscribing in Start rather than OnEnable because the service arrives
        // through injection at the scope's Awake, which is not ordered against this
        // component's own Awake.
        private void Start()
        {
            _upgrades.Changed += Apply;
            Apply();
        }

        private void OnDestroy()
        {
            _upgrades.Changed -= Apply;
            StopIntro();
        }

        private void Apply()
        {
            // Answering one card can roll the next in the same frame, so a running
            // intro is always torn down before the panel is rewritten.
            StopIntro();

            if (!_upgrades.IsChoosing)
            {
                panel.SetActive(false);
                return;
            }

            IReadOnlyList<UpgradeOption> offer = _upgrades.Offer;

            for (int i = 0; i < cards.Length; i++)
            {
                bool used = i < offer.Count;
                cards[i].button.gameObject.SetActive(used);

                if (!used)
                    continue;

                cards[i].frame.color = offer[i].tint;
                cards[i].icon.sprite = offer[i].icon;
                cards[i].category.SetText(offer[i].category);
                cards[i].stat.SetText(offer[i].statName);
                cards[i].value.SetText(offer[i].ValueLabel);

                ApplyIntro(cards[i], 0f);
            }

            panel.SetActive(true);

            _introCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            PlayIntro(offer.Count, _introCts.Token).Forget();
        }

        private void StopIntro()
        {
            if (_introCts == null)
                return;

            _introCts.Cancel();
            _introCts.Dispose();
            _introCts = null;
        }

        // Yield rather than Delay, and unscaled delta on top: the panel only ever
        // opens with Time.timeScale at zero, so anything riding the scaled clock
        // would sit still exactly when it is meant to play.
        private async UniTask PlayIntro(int count, CancellationToken token)
        {
            float elapsed = 0f;
            float total = introStagger * Mathf.Max(count - 1, 0) + introDuration;

            while (elapsed < total)
            {
                bool cancelled = await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow();

                if (cancelled)
                    return;

                elapsed += Time.unscaledDeltaTime;

                for (int i = 0; i < count && i < cards.Length; i++)
                    ApplyIntro(cards[i], Mathf.Clamp01((elapsed - i * introStagger) / introDuration));
            }

            for (int i = 0; i < count && i < cards.Length; i++)
                ApplyIntro(cards[i], 1f);
        }

        // Alpha eases up to one while the card swells and settles back. Sin over half
        // a period returns to exactly 1, so a cancelled intro can never leave a card
        // stuck at a punched scale.
        private void ApplyIntro(Card card, float progress)
        {
            card.group.alpha = Mathf.SmoothStep(0f, 1f, progress);
            card.button.transform.localScale = Vector3.one * (1f + introPunch * Mathf.Sin(progress * Mathf.PI));
        }
    }
}
