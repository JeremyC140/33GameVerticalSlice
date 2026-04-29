using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public bool isSongPlaying = false;
    private AudioSource _audioSource;

    [Range(0f, 1f)]
    public float musicVolume = 0.8f;
    

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = musicVolume;
    }
    void OnEnable()
    {
        // --- Event Subscription ---
        GameController.OnPauseGame += HandlePause;
        GameController.OnRestartGame += HandleRestart;
        GameController.OnQuitGame += HandleQuit;
    }

    void OnDisable()
    {
        // --- Event Unsubscription ---
        GameController.OnPauseGame -= HandlePause;
        GameController.OnRestartGame -= HandleRestart;
        GameController.OnQuitGame -= HandleQuit;
    }

    private void Update()
    {
        isSongPlaying = _audioSource.isPlaying;
    }

    public void PlaySong(SongData song, double startTime)
    {
        _audioSource.clip = song.audioFile;
        _audioSource.PlayScheduled(startTime);
    }

    private void HandlePause()
    {
        if (_audioSource.isPlaying) { 
            _audioSource.Pause();
        }
        else {
            _audioSource.UnPause();
        }
    }

    private void HandleRestart()
    {
        _audioSource.Stop();
        _audioSource.Play();
    }

    private void HandleQuit()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
    }
}
