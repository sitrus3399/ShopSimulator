using PalmVilleAudio;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private List<string> selectedRoute;
    [SerializeField] private GameEvents gameEvent;

    public List<string> SelectedRoute { get { return selectedRoute; } }

    private void Start()
    {
        gameEvent.OnGameOver += RestartGame;

        AudioManager.Instance.PlayBGM("MainBGM");
    }

    public void addSelectedRoute(string newRoute)
    {
        selectedRoute.Add(newRoute);
    }

    public void RestartGame()
    {
        AudioManager.Instance.StopAllBGM();
        AudioManager.Instance.StopAllSFX();
        AudioManager.Instance.StopAllVoice();
        AudioManager.Instance.StopAllVideoSound();
        AudioManager.Instance.StopAllNotif();
    }
}