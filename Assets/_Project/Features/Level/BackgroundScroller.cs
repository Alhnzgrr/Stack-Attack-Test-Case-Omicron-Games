using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Level
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BackgroundScroller : MonoBehaviour
    {
        [SerializeField] private float parallaxFactor = 1f;

        private Material _material;
        private LevelProgress _progress;
        private float _unitsPerTile;

        [Inject]
        public void Construct(LevelProgress progress)
        {
            _progress = progress;
        }

        private void Awake()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            // Reading material rather than sharedMaterial keeps the asset on disk untouched.
            _material = meshRenderer.material;

            // Derived from the quad and the material so rescaling or retiling the
            // background can never drift out of sync with the stacks.
            _unitsPerTile = meshRenderer.bounds.size.y / _material.mainTextureScale.y;
        }

        private void Update()
        {
            float offset = _progress.Travelled * parallaxFactor / _unitsPerTile;

            _material.mainTextureOffset = new Vector2(0f, offset);
        }
    }
}
