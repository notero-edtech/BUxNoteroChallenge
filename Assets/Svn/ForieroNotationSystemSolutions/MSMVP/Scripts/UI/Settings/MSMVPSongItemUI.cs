using ForieroEngine.Music.NotationSystem.Classes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MSMVPSongItemUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Button playButton;
    public Session session;
    public MSMVPSongsUI songs;

    private void Awake()
    {
        playButton.onClick.AddListener(() => songs.Play(session));
    }
}
