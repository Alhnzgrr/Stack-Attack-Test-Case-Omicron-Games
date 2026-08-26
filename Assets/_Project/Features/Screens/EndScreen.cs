using System.Threading;
using Cysharp.Threading.Tasks;
using StackAttack.Core;
using TMPro;
using UnityEngine;
using VContainer;

namespace StackAttack.Screens
{
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI hintLabel;

        [SerializeField] private float titleDuration = 0.45f;
        [SerializeField] private float titleEntryScale = 0.6f;
        [SerializeField] private float titlePunch = 0.14f;
        [SerializeField] private float hintDelay = 0.25f;
        [SerializeField] private float hintDuration = 0.3f;

        private IPointerInput _input;
        private GameStateMachine _state;

        private CancellationTokenSource _introCts;
        private bool _ready;

        [Inject]
        public void Construct(IPointerInput input, GameStateMachine state)
        {
            _input = input;
            _state = state;
        }

        private void OnEnable()
        {
            bool won = _state.Current == GameState.Won;

            titleLabel.SetText(won ? "Level Complete" : "Game Over");
            hintLabel.SetText(won ? "Tap for the next level" : "Tap to try again");

            // The panel comes up on the same frame the run ended, with the player
            // still tapping at whatever they were shooting. Nothing is listening
            // until the hint is on screen to say it is.
            _ready = false;

            ApplyTitle(0f);
            ApplyHint(0f);

            _introCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            PlayIntro(_introCts.Token).Forget();
        }

        private void OnDisable()
        {
            StopIntro();
        }

        private void OnDestroy()
        {
            StopIntro();
        }

        private void Update()
        {
            if (_ready && _input.PressedThisFrame)
                _state.Set(GameState.Playing);
        }

        private void StopIntro()
        {
            if (_introCts == null)
                return;

            _introCts.Cancel();
            _introCts.Dispose();
            _introCts = null;
        }

        // The title lands first and the hint follows it, so the screen reads as a
        // result before it reads as a button. Unscaled delta because a run can end
        // out of the upgrade panel, which leaves the scaled clock stopped.
        private async UniTask PlayIntro(CancellationToken token)
        {
            float elapsed = 0f;

            while (elapsed < titleDuration)
            {
                bool cancelled = await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow();

                if (cancelled)
                    return;

                elapsed += Time.unscaledDeltaTime;
                ApplyTitle(Mathf.Clamp01(elapsed / titleDuration));
            }

            ApplyTitle(1f);

            elapsed = 0f;

            while (elapsed < hintDelay + hintDuration)
            {
                bool cancelled = await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow();

                if (cancelled)
                    return;

                elapsed += Time.unscaledDeltaTime;
                ApplyHint(Mathf.Clamp01((elapsed - hintDelay) / hintDuration));
            }

            ApplyHint(1f);

            _ready = true;
        }

        // The title swells past its own size on the way in and settles back. Sin over
        // half a period returns to exactly zero, so the punch cannot leave the label
        // stuck at a size it was only passing through.
        private void ApplyTitle(float progress)
        {
            float eased = Mathf.SmoothStep(0f, 1f, progress);

            titleLabel.alpha = eased;
            titleLabel.transform.localScale =
                Vector3.one * (Mathf.LerpUnclamped(titleEntryScale, 1f, eased) + titlePunch * Mathf.Sin(progress * Mathf.PI));
        }

        private void ApplyHint(float progress)
        {
            hintLabel.alpha = Mathf.SmoothStep(0f, 1f, progress);
        }
    }
}
