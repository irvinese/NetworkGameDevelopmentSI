using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.VisualScripting;
using System;
using UnityEngine.SceneManagement;

public class Lobby_Manager : NetworkBehaviour
{
    //player lable improvised
   [SerializeField] private Button _startBttn, _leaveBttn, _readyBttn;
   [SerializeField] private GameObject _panelPrefab;
   [SerializeField] private GameObject _ContentGO;
   [SerializeField] private TMP_Text rdyTxt;

   [SerializeField] private NetworkPlayerData _networkPlayers;

   
   private List<GameObject> _PlayerPanels = new List<GameObject>();
   private ulong _myServerID;
   private bool isReady = false;

   private void Start()
   {
    _myServerID = NetworkManager.ServerClientId;

    if (IsServer)
    {
        rdyTxt.text = "Waiting for Players";
        _readyBttn.gameObject.SetActive(false);
    }
    else{
        rdyTxt.text = "Not Ready";
        _readyBttn.gameObject.SetActive(true);
    }

    if (_networkPlayers == null)
        {
            Debug.LogError("NetworkPlayerData is not assigned in the Lobby_Manager.");
            return;
        }

    _networkPlayers._allConnectedPlayer.OnListChanged += NetPlayersChanged;
    _leaveBttn.onClick.AddListener(LeaveBttnClick);
    _readyBttn.onClick.AddListener(ClientRdyBttnToggle);
   }

    private void ClientRdyBttnToggle()
    {
        if(IsServer){return;}

        isReady = !isReady;
        if (isReady)
        {
            rdyTxt.text = "Ready";
        }
        else{
            rdyTxt.text = "Not Ready";
        }

        RdyBttnToggleServerRpc(isReady);
    }


    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void RdyBttnToggleServerRpc(bool readyStatus, RpcParams rpcParams = default)
    {
        var senderId = rpcParams.Receive.SenderClientId;
        Debug.Log("rdybttntoggleserverRpc called by client: " + senderId);

        Debug.Log(message: "From Rdy bttn RPC");
        _networkPlayers.UpdateReadyClient(rpcParams.Receive.SenderClientId, readyStatus);
    }

    private void LeaveBttnClick()
    {
        if(!IsServer)
        {
            QuitLobbyServerRpc();
        }
        else{
            foreach(PlayerInfoData playerdata in _networkPlayers._allConnectedPlayer)
            {
                KickUserButton(playerdata._clientId);
            }
        }
        NetworkManager.Shutdown();
    }

    [Rpc(target: SendTo.Server, RequireOwnership = false)]
    private void QuitLobbyServerRpc(RpcParams rpcParams=default)
    {
        KickUserButton(rpcParams.Receive.SenderClientId);
    }

    private void NetPlayersChanged(NetworkListEvent<PlayerInfoData> changeEvent)
    {
        Debug.Log("Net players has changed event fired!");
        PopulateLabels();
    }

    [ContextMenu(itemName: "PopulateLabel")]
   private void PopulateLabels()
   {
    ClearPlayerPanel();

    bool allReady = true;

    if (_networkPlayers._allConnectedPlayer.Count == 0)
        {
            Debug.LogWarning("No connected players to populate.");
            return; // Exit early if there are no players
        }

    foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayer)
    {

        if (playerData == null)
            {
                Debug.LogWarning("Found null player data.");
                continue; // Skip null player data
            }

        GameObject newPlayerPanel = Instantiate(_panelPrefab, _ContentGO.transform);
        PlayerLabel _playerLabel = newPlayerPanel.GetComponent<PlayerLabel>();

        if (_playerLabel == null)
            {
                Debug.LogError("PlayerLabel component is missing from the panel prefab.");
                Destroy(newPlayerPanel); // Clean up the failed panel
                continue; // Skip further processing for this player
            }

        _playerLabel.onKickClicked += KickUserButton;

        if(IsServer && playerData._clientId != _myServerID)
        {
            _playerLabel.SetKickActive(isOn: true);

            _readyBttn.GameObject().SetActive(false);
        }
        else{
            _playerLabel.SetKickActive(isOn: false);
        }

        _playerLabel.SetPlayerLabelName(playerData._clientId);
        _playerLabel.SetReady(playerData._isPlayerReady);
        _playerLabel.SetPlayerColor(playerData._colorId);
        _PlayerPanels.Add(newPlayerPanel);

        if(playerData._isPlayerReady == false)
        {
            allReady = false;
        }

        if(IsServer)
        {
            if (allReady)
            {
                if (_networkPlayers._allConnectedPlayer.Count > 1)
                {
                    rdyTxt.text = "Ready to start";
                    _startBttn.gameObject.SetActive(true);
                }
                else{
                    rdyTxt.text = "Empty Lobby";
                }
            }
            else{
                _startBttn.gameObject.SetActive(false);
                rdyTxt.text = "Waiting for ready players";
            }
        }
        

    }
    
   }

   private void KickUserButton(ulong kickTarget)
   {
        if(!IsServer || !IsHost) return;

        foreach(PlayerInfoData playerData in _networkPlayers._allConnectedPlayer)
        {
            if(playerData._clientId == kickTarget)
            {
                //_networkPlayers._allConnectedPlayers.Remove(playerData);

                KickedClientRpc(RpcTarget.Single(kickTarget, RpcTargetUse.Temp));

                NetworkManager.Singleton.DisconnectClient(kickTarget);
            }
        }
   }

     [Rpc(target: SendTo.SpecifiedInParams)]
    private void KickedClientRpc(RpcParams rpcParams)
    {
        SceneManager.LoadScene(0);
    }
    private void ClearPlayerPanel()
   {
       foreach (GameObject panel in _PlayerPanels)
       {
        Destroy(panel);
       }

       _PlayerPanels.Clear();
   }
}
