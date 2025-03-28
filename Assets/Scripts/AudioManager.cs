using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get { return _instance; }
    }

    [SerializeField] private AudioSource _clipAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }

    public void PlaySoundAtPlayer(AudioClip clip)
    {
        _clipAudioSource.clip = clip;
        _clipAudioSource.Play();
    }
}
