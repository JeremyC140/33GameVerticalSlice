using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SongSelectionManager : MonoBehaviour
{
    [Header("UI References (Right Column)")]
    public TextMeshProUGUI detailTitle;
    public TextMeshProUGUI detailArtist;
    public Image detailAlbumArt;
    public TextMeshProUGUI detailBPM;
    public TextMeshProUGUI detailDifficulty;

    [Header("Scroller Setup (Left Column)")]
    public Transform contentParent; // Drag the 'Content' object here
    public GameObject songButtonPrefab;
    public SongData[] allSongs; // Drag all your SongData assets here

    public SongData _selectedSong;

    void Start()
    {
        PopulateList();
    }

    void PopulateList()
    {
        foreach (SongData song in allSongs)
        {
            GameObject btnObj = Instantiate(songButtonPrefab, contentParent);
            SongButtonDisplay btnScript = btnObj.GetComponent<SongButtonDisplay>();
            btnScript.Setup(song, this);
        }
    }

    public void SelectSong(SongData data)
    {
        _selectedSong = data;

        // Update the Right Column UI
        detailTitle.text = data.songName;
        detailArtist.text = data.artist;
        detailBPM.text = $"BPM: {data.songBPM}";
        detailDifficulty.text = $"Difficulty: {data.difficultyLevel}";
        detailAlbumArt.sprite = data.songImage;
    }

    public void PlaySelectedSong()
    {
        if (_selectedSong != null)
        {
            // Store the selected song in the GameController singleton 
            GameController.Instance.currentSong = _selectedSong;
            SceneController.Instance.LoadScene("GamePlayScene");
        }
    }
}