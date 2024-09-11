using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Xml.Serialization;

public class UI_net_Manager : NetworkBehaviour
{
    [SerializeField] private Button _StartServerbutton, _Starthostbutton, _StartClientbutton, _StartGameButton;
    [SerializeField] private GameObject _connectionButtonGroup;

    [SerializeField] private Spawn_Controller _mySpawnController;

    void Start()
    {
        _StartGameButton.gameObject.SetActive(false);
       if (_StartServerbutton != null) _StartServerbutton.onClick.AddListener(ServerStartClick);
       if (_StartClientbutton != null) _StartClientbutton.onClick.AddListener(ClientStartClick);
       if (_Starthostbutton != null) _Starthostbutton.onClick.AddListener(HostStartClick);
       if (_StartGameButton != null) _StartGameButton.onClick.AddListener(GameStartClick);
    }
 
   private void ServerStartClick()
   {
    NetworkManager.Singleton.StartServer();
    _connectionButtonGroup.SetActive(false);
    _StartGameButton.gameObject.SetActive(true);
   }

    private void HostStartClick()
   {
    NetworkManager.Singleton.StartHost();
    _connectionButtonGroup.SetActive(false);
    _StartGameButton.gameObject.SetActive(true);
   }

   private void ClientStartClick()
   {
    NetworkManager.Singleton.StartClient();
    _connectionButtonGroup.SetActive(false);
   }

   private void GameStartClick()
   {
    if(IsServer)
    {
      _mySpawnController.SpawnAllPlayers();
      _StartGameButton.gameObject.SetActive(false);
    }
   }
}
