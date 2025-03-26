using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get { return _instance; }
    }

    private AudioSource _audioSource;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(this);
    }

    private void Start()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
    }


    public void PlaySoundAtPlayer(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
