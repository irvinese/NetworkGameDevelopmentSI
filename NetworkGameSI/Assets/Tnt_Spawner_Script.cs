using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Tnt_Spawner_Script : NetworkBehaviour
{
    public NetworkObject TNT;

    [SerializeField] private float tickTime = 6f;
    [SerializeField] private float currentTime = 6f;

    public void FixedUpdate()
    {
        // Ensure only the server handles TNT spawning
        if (!IsServer) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            SpawnTNT();
            currentTime = Random.Range(tickTime, tickTime + 5f);
        }
    }

    private void SpawnTNT()
    {
        // Instantiate TNT and spawn it across the network
        NetworkObject tntInstance = Instantiate(TNT, transform.position, transform.rotation);
        tntInstance.Spawn(destroyWithScene: true);
    }
}

