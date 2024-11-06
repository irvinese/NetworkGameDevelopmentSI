using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private Spawn_Controller _spawnController;
    public static GameStateManager Instance;

    // Assign the SpawnController in the Inspector or find it dynamically
    void Awake()
    {
        _spawnController = FindObjectOfType<Spawn_Controller>();  // Finds the Spawn_Controller in the scene
    }

    // Set the flag to true when ready to spawn
    public void SetPlayersReadyToSpawn(bool ready)
    {
        // Set some condition in Spawn_Controller
        if (ready)
        {
            // Call SpawnAllPlayers from Spawn_Controller
            _spawnController.SpawnAllPlayers();
        }
    }
}


