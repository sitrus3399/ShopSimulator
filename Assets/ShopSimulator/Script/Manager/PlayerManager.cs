using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerUI playerUIPrefab;
    private PlayerUI playerUI;
    
    [SerializeField] private PlayerController playerPrefab;
    private PlayerController playerController;
    [SerializeField] private Transform playerSpawnLocation;

    private void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        playerUI = Instantiate(playerUIPrefab);
        playerController = Instantiate(playerPrefab, playerSpawnLocation.transform.position, playerSpawnLocation.transform.rotation);
    }
}