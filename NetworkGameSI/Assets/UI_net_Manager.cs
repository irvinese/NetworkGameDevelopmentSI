using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Xml.Serialization;

public class UI_net_Manager : NetworkBehaviour
{
    [SerializeField] private Button _StartServerbutton, _Starthostbutton, _StartClientbutton;

    void Start()
    {
        _StartServerbutton.onClick.AddListener(ServerStartClick);
        _StartClientbutton.onClick.AddListener(ClientStartClick);
        _Starthostbutton.onClick.AddListener(HostStartClick);
    }
 
   private void ServerStartClick()
   {
    NetworkManager.Singleton.StartServer();
    this.gameObject.SetActive(false);
   }

    private void HostStartClick()
   {
    NetworkManager.Singleton.StartHost();
    this.gameObject.SetActive(false);
   }

   private void ClientStartClick()
   {
    NetworkManager.Singleton.StartClient();
    this.gameObject.SetActive(false);
   }
}
