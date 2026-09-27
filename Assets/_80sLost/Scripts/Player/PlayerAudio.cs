using UnityEngine;

namespace Lost80s
{
    /// <summary>
    /// Central 2D (non-positional) audio output on the player. Interaction/event
    /// sounds route through here instead of playing from the world object that
    /// caused them, so they stay at full volume after the player walks away.
    /// </summary>
    public class PlayerAudio : MonoBehaviour
    {
        private static PlayerAudio _instance;

        [SerializeField] private AudioSource sfxSource;

        private void Awake()
        {
            _instance = this;
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;
            sfxSource.loop = false;
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        public static void Play(AudioClip clip, float volume = 1f)
        {
            if (clip == null || _instance == null) return;
            _instance.sfxSource.PlayOneShot(clip, volume);
        }
    }
}
