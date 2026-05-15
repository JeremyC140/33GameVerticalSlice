using UnityEngine;
using TMPro;

public class SongButtonDisplay : MonoBehaviour
{
    public SongData songData;
    public TextMeshProUGUI songTitle;

    void Start()
    {
        if (songData != null && songTitle != null)
        {
            songTitle.text = songData.songName;
        }
    }

    public void Setup(SongData data, SongSelectionManager manager)
    {
        songData = data;
        if (songTitle != null && songData != null)
        {
            songTitle.text = songData.songName;
        }
    }

    public void onClicked()
    {
        if (songData != null)
        {
            SongSelectionManager manager = FindAnyObjectByType<SongSelectionManager>();
            if (manager != null)
            {
                manager.SelectSong(songData);
            }
        }
    }
}