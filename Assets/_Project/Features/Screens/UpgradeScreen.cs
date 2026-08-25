using System;
using System.Collections.Generic;
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
            public Image frame;
            public Image icon;
            public TextMeshProUGUI category;
            public TextMeshProUGUI stat;
            public TextMeshProUGUI value;
        }

        [SerializeField] private GameObject panel;
        [SerializeField] private Card[] cards;

        private IUpgradeService _upgrades;

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
        }

        private void Apply()
        {
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
            }

            panel.SetActive(true);
        }
    }
}
