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
            public TextMeshProUGUI title;
            public TextMeshProUGUI description;
        }

        [SerializeField] private GameObject panel;
        [SerializeField] private Card[] cards;

        private IUpgradeService _upgrades;
        private int _shownOfferId = -1;

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

        // The service opens the choice from inside a collision callback, and answering
        // one can roll the next in the same frame, so the panel follows the offer id
        // rather than a single event.
        private void Update()
        {
            if (!_upgrades.IsChoosing)
            {
                if (_shownOfferId < 0)
                    return;

                _shownOfferId = -1;
                panel.SetActive(false);
                return;
            }

            if (_shownOfferId == _upgrades.OfferId)
                return;

            _shownOfferId = _upgrades.OfferId;
            Render();
        }

        private void Render()
        {
            IReadOnlyList<UpgradeOption> offer = _upgrades.Offer;

            for (int i = 0; i < cards.Length; i++)
            {
                bool used = i < offer.Count;
                cards[i].button.gameObject.SetActive(used);

                if (!used)
                    continue;

                cards[i].frame.color = offer[i].tint;
                cards[i].title.SetText(offer[i].title);
                cards[i].description.SetText(offer[i].description);
            }

            panel.SetActive(true);
        }
    }
}
