using StackAttack.Core;
using UnityEngine;
using VContainer;

namespace StackAttack.Playfield
{
    [RequireComponent(typeof(Camera))]
    public class PlayfieldCamera : MonoBehaviour
    {
        private Camera _camera;
        private PlayfieldConfig _playfield;

        private int _width;
        private int _height;

        [Inject]
        public void Construct(PlayfieldConfig playfield)
        {
            _playfield = playfield;
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            Apply();
        }

        // A resize has nothing to announce it and a rotation changes the aspect
        // mid-run, so this is the case where reading the value every frame is the
        // honest shape rather than a missing event.
        private void Update()
        {
            if (Screen.width == _width && Screen.height == _height)
                return;

            Apply();
        }

        // The playfield width is what levels are authored against, so it is the
        // measure that stays fixed. A taller screen sees further ahead instead of
        // squeezing the field, which is what kept the player half off the edge.
        private void Apply()
        {
            _width = Screen.width;
            _height = Screen.height;

            _camera.orthographicSize = _playfield.HalfWidth / _camera.aspect;
        }
    }
}
