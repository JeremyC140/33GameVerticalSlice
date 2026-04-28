using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;

    [Range(0f, 1f)]
    public float musicVolume = 0.8f;

    void Awake()
    {
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

    public void PlaySong(SongData song)
    {
        _audioSource.clip = song.audioFile;
        _audioSource.Play();
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
