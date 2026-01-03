using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Events/Game", order = 0)]
public class GameEvents : ScriptableObject
{
    public Action OnGameOver;
    public Action OnGameWon;

    public void GameOver()
    {
        OnGameOver?.Invoke();
    }

    public void GameWon() 
    {
        OnGameWon?.Invoke();
    }
}