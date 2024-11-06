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
    private NetworkVariable<int> _playerCount = new NetworkVariable<int>(value: 0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField]
    private TMP_Text _countTxt;

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

 
    [Rpc(target: SendTo.Everyone)]
    private void UpdateCountTextClientRpc(int newValue)
    {
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
        if (!IsServer) return;

    int spawnNum = 0;
    foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
    {

        NetworkObject playerObject = Instantiate(_playerPrefab, _spawnPoints[spawnNum].position, _spawnPoints[spawnNum].rotation);
        
        if (playerObject != null)
        {
            playerObject.SpawnAsPlayerObject(clientId);
        }
        
        spawnNum++;
    }
    }


}
