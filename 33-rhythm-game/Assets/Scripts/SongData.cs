using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSong", menuName = "RhythmGame/SongData")]
public class SongData : ScriptableObject
{
    [Header("General Info")]
        public string songName;
        public string artist;

    [Header("Audio Files")]
        public AudioClip audioFile;

    [Header("Metadata")]
        public float songBPM;

        [Tooltip("The time in seconds before the first beat. Essential for syncing notes to audio.")]
        public float offset;

        [Tooltip("When the preview music starts in the menu (seconds).")]
        public float previewStartTime;

    [Header("Difficulty")]
        public int difficultyLevel;

    [Header("Chart Data")]
        public List<NoteData> chart;

    // A helper function to get length directly from the clip
    public float GetSongLength()
    {
        return audioFile != null ? audioFile.length : 0f;
    }

    // Automatically sorts the notes by time so the spawner doesn't get confused
    [ContextMenu("Sort Chart by Time")]
    public void SortNotes()
    {
        chart.Sort((a, b) => a.hitTime.CompareTo(b.hitTime));
    }
}

[System.Serializable]
public struct NoteData
{
    public double hitTime; // The exact dspTime the note should be hit

    [Range(1, 7)]
    public int laneIndex; // 1=R, 2=F, 3=V, 4=Space, 5=I, 6=J, 7=N

    public NoteType type;
}

public enum NoteType
{
    Tap,
    Hold,
    Slide
}
