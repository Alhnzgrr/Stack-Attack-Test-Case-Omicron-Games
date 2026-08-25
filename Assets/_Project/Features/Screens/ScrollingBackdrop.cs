using UnityEngine;
using UnityEngine.UI;

namespace StackAttack.Screens
{
    [RequireComponent(typeof(Image))]
    public class ScrollingBackdrop : MonoBehaviour
    {
        private static readonly int PhaseId = Shader.PropertyToID("_Phase");
        private static readonly int AspectId = Shader.PropertyToID("_Aspect");

        private RectTransform _rect;
        private Material _material;

        private void Awake()
        {
            _rect = (RectTransform)transform;

            Image image = GetComponent<Image>();

            // Instanced so pushing the phase every frame does not dirty the shared
            // material asset while the editor has it open.
            _material = new Material(image.material);
            image.material = _material;

            ApplyAspect();
        }

        private void OnDestroy()
        {
            Destroy(_material);
        }

        // Unity raises this exactly when the rect is resized, which is the only thing
        // that can change the aspect. Layout can fire it before Awake has built the
        // material, hence the check.
        private void OnRectTransformDimensionsChange()
        {
            if (_material != null)
                ApplyAspect();
        }

        // The panel is only ever on screen while Time.timeScale is zero, and that
        // freezes the shader's own _Time as well. Driving the phase from unscaled
        // time is what keeps the background moving exactly when it is visible.
        private void Update()
        {
            _material.SetFloat(PhaseId, Time.unscaledTime * 2f);
        }

        private void ApplyAspect()
        {
            float height = _rect.rect.height;

            if (height > 0f)
                _material.SetFloat(AspectId, _rect.rect.width / height);
        }
    }
}
