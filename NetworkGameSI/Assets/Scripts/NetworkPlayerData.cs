using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Xml.Serialization;
using System;

public class NetworkPlayerData : NetworkBehaviour
{
   public NetworkList<PlayerInfoData> _allConnectedPlayer;
   private int _players = -1;
   private ulong _serverLocalID;

   private Color[] _playerColors = new Color[]
   {
    Color.blue, Color.magenta, Color.cyan, Color.yellow, Color.white
   };


   private void Awake()
   {
    _allConnectedPlayer = new NetworkList<PlayerInfoData>(readPerm: NetworkVariableReadPermission.Everyone);
   }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(!IsServer) return;
        NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvents;
        _serverLocalID = NetworkManager.ServerClientId;

        Debug.Log("OnNetworkSpawn triggered.");
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvents;
        }
        base.OnNetworkDespawn();
    }

    private void OnConnectionEvents(NetworkManager netManager, ConnectionEventData eventData)
    {
        if (eventData.EventType == ConnectionEvent.ClientConnected)
        {
            CreateNewClientData(eventData.ClientId);
        }

        if(eventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            RemovePlayerData(FindPlayerInfoData(eventData.ClientId));
            _players--;
        }

        Debug.Log("player Connections Triggered");
    }

    private void CreateNewClientData(ulong clientID)
   {
    PlayerInfoData playerInfoData = new PlayerInfoData(clientID);

    if (_serverLocalID == clientID)
    {
        playerInfoData._isPlayerReady = true;
    }
    else{
        playerInfoData._isPlayerReady = false;
    }

    _players++;

    playerInfoData._colorId = _playerColors[_players];

    _allConnectedPlayer.Add(playerInfoData);
   }

   public void RemovePlayerData(PlayerInfoData playerData)
   {
    _allConnectedPlayer.Remove(playerData);
   }

   public PlayerInfoData FindPlayerInfoData(ulong clientID)
   {
    return  _allConnectedPlayer[FindPlayerIndex(clientID)];
   }

  private int FindPlayerIndex(ulong clientId)
{
    List<PlayerInfoData> safeListCopy = new List<PlayerInfoData>();
    foreach (var playerInfo in _allConnectedPlayer)
    {
        safeListCopy.Add(playerInfo);
    }

    for (int i = 0; i < safeListCopy.Count; i++)
    {
        if (safeListCopy[i]._clientId == clientId)  // Use _clientId instead of ClientId
        {
            return i;
        }
    }

    return -1; // Return -1 if no matching player is found
}



   public void UpdateReadyClient(ulong clientID, bool isReady)
   {
    int idx = FindPlayerIndex(clientID);
    if (idx == -1){return;}

    PlayerInfoData playerInfo = new PlayerInfoData();

    playerInfo = _allConnectedPlayer[idx];

    playerInfo._isPlayerReady = isReady;

    _allConnectedPlayer[idx] = playerInfo;

    Debug.Log("Readiness Update");
   }
}
