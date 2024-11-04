using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Unity.Netcode;

public class Lobby_Manager : NetworkBehaviour
{
   [SerializeField] private Button _startButton, _leaveButton, _readyButton;
   [SerializeField] private GameObject _panelPrefab;
   [SerializeField] private GameObject _ContentGO;
   [SerializeField] private TMP_Text rdyTxt;

   
   private List<GameObject> _PlayerPanels = new List<GameObject>();
   private ulong _myLocalClientID;
   private bool isReady = false;
}
