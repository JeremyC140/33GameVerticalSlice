using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultMenuManager : MonoBehaviour
{
    [Header("UI References (Right Column)")]
    public TextMeshProUGUI perfectCount;
    public TextMeshProUGUI goodCount;
    public TextMeshProUGUI missCount;
    public TextMeshProUGUI accuracy;

    [Header("Image Setup (Left Column)")]
    public Image songImage;

    public SongData selectedSong;

    void Start() { 
        perfectCount.text = "Perfect: " + ValueKeeper.Instance.perfectCount.ToString();
        goodCount.text = "Good: " + ValueKeeper.Instance.goodCount.ToString();
        missCount.text = "Miss: " + ValueKeeper.Instance.missCount.ToString();
        accuracy.text = "Accuracy: " + (ValueKeeper.Instance.accuracy * 100).ToString("F2") + "%";
        selectedSong = ValueKeeper.Instance.chosenSong;
        songImage.sprite = selectedSong.songImage;
    }

    public void returnToMenu() { 
        SceneController.Instance.LoadScene("SelectionMenu");
    }
}   
