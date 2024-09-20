using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class Spawn_Controller : NetworkBehaviour
{

    [SerializeField]
    private NetworkObject _playerPrefab;

    [SerializeField]
    private Transform[] _spawnPoints;

    [SerializeField]
    NetworkVariable<int> _playerCount = new NetworkVariable<int>(value: 0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField]
    private TMP_Text _countTxt;
    private int newValue;

    public override void OnNetworkSpawn()
    {

        base.OnNetworkSpawn();

        //NetworkManager.Singleton.OnClientConnectedCallback += OnConnectionEvent;

        if(IsServer)
        {
        NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
        }

        _playerCount.OnValueChanged += PlayerCountChange;

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn(); 

        if(IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
        }

        _playerCount.OnValueChanged -= PlayerCountChange;
    }

    private void PlayerCountChange(int previousValue, int newValue)
    {
        UpdateCountTextClientRpc(newValue);
    }

/* old
    [ClientRpc]
    private void UpdateCountTextClientRpc()
    {

    }

    [ServerRpc]
    private void SomeServerRpc()
    {

    }
    */

[Rpc(target:SendTo.Everyone)]
private void UpdateCountTextClientRpc(int newValue)
{
    Debug.Log(message: "Message From Client RPC");
    UpdateCountText(newValue);
}


    private void UpdateCountText(int newValue)
    {
        _countTxt.text = $"Players : {newValue}";
    }

    private void OnConnectionEvent(NetworkManager netManager, ConnectionEventData eventData)
    {
        if(eventData.EventType == ConnectionEvent.ClientConnected)
        {
            _playerCount.Value++;
        }
    }

    public void SpawnAllPlayers()
    {
        if(!IsServer) return;

        int spawnNum = 0;
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            NetworkObject spawnedPlayerNO = NetworkManager.Instantiate(_playerPrefab, _spawnPoints[spawnNum].position, _spawnPoints[spawnNum].rotation);

           spawnedPlayerNO.SpawnAsPlayerObject(clientId);
           
            spawnNum++;
        }
    }

}
