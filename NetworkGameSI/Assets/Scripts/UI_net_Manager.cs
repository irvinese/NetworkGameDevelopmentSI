using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Xml.Serialization;
using System;
using TMPro;
using Unity.VisualScripting;

public class UI_net_Manager : NetworkBehaviour
{
    [SerializeField] private Button _StartServerbutton, _Starthostbutton, _StartClientbutton, _StartGameButton;
    [SerializeField] private GameObject _connectionButtonGroup, _socialPanel;
    [SerializeField] private Spawn_Controller _mySpawnController;

    [SerializeField] private Button _SendButton;
    [SerializeField] TMP_InputField _InputField;
    [SerializeField] private GameObject _messagePrefab;
    [SerializeField] private Transform _ScrollViewContent;

    void Start()
    {

      if (_mySpawnController == null)
        {
            _mySpawnController = FindObjectOfType<Spawn_Controller>();
        }

        _StartGameButton.gameObject.SetActive(false);
       if (_StartServerbutton != null) _StartServerbutton.onClick.AddListener(ServerStartClick);
       if (_StartClientbutton != null) _StartClientbutton.onClick.AddListener(ClientStartClick);
       if (_Starthostbutton != null) _Starthostbutton.onClick.AddListener(HostStartClick);
       if (_StartGameButton != null) _StartGameButton.onClick.AddListener(GameStartClick);

       if (_SendButton != null) _SendButton.onClick.AddListener(SendMessage);
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

      HideGuiRpc();
    }
    Debug.Log("Game Start");
   }

  [Rpc(target: SendTo.Everyone)]
    private void HideGuiRpc()
    {
      if (_socialPanel != null)
      {
        _socialPanel.SetActive(false);
        Debug.Log("HideGuiRpc Executed, hiding social panel");
      }
      else{
        Debug.Log("socialpanel is null on client");
      }

        Debug.Log("Executed correctlty" + NetworkManager.Singleton.LocalClientId);
    }

    private void SendMessage()
    {
      string message = _InputField.text.Trim();
      if (!string.IsNullOrEmpty(message))
      {
        UpdateSocialTextServerRPC(message);
        _InputField.text = "";
      }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void UpdateSocialTextServerRPC(string message)
    {
      UpdateSocialTextClientRPC(message);
    }

    [Rpc(target: SendTo.Everyone)]
    private void UpdateSocialTextClientRPC(string message)
    {
      if (_ScrollViewContent != null && _messagePrefab != null)
      {
        GameObject newMessage = Instantiate(_messagePrefab, _ScrollViewContent);

        TMP_Text messageTextComponent = newMessage.GetComponent<TMP_Text>();
        if(messageTextComponent != null)
        {
          messageTextComponent.text = message;
        }
      }
    }
}
