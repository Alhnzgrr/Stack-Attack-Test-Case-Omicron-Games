using UnityEngine;

namespace StackAttack.Audio
{
    // The voices live on this one object and are handed out in turn, so a sound never
    // cuts the one before it off the way a single shared source would.
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] private int voices = 8;

        private AudioSource[] _sources;
        private int _next;

        private void Awake()
        {
            _sources = new AudioSource[Mathf.Max(voices, 1)];

            for (int i = 0; i < _sources.Length; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();

                source.playOnAwake = false;
                source.loop = false;
                source.spatialBlend = 0f;

                _sources[i] = source;
            }
        }

        public void Play(AudioClip clip, float volume, float pitch)
        {
            AudioSource source = _sources[_next];

            _next = (_next + 1) % _sources.Length;

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
        }
    }
}
