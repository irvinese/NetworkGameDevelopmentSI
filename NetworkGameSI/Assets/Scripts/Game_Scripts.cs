using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;

namespace App.Resource.Scripts
{
public class Game_Scripts : NetworkBehaviour
{
    [SerializeField] private List<ulong> _currentPlayers;
    
    public TMP_Text myWinText;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            myWinText.gameObject.SetActive(false);
        }

        [Rpc(target: SendTo.Server)]
        public void AddPlayersRpc(RpcParams rpcParams = default)
        {
            _currentPlayers.Add(rpcParams.Receive.SenderClientId);
        }
        public void AddPlayerToList(ulong playerID)
            {
             if (!_currentPlayers.Contains(playerID))
                {
                _currentPlayers.Add(playerID);
                 }
            }


        [Rpc(target: SendTo.Server)]
        public void PlayerDeathRpc(RpcParams rpcParams= default)
        {
            _currentPlayers.Remove(rpcParams.Receive.SenderClientId);
            YouLoseRpc((RpcParams) RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));

            if (_currentPlayers.Count == 1)
            {
                YouWinRpc((RpcParams) RpcTarget.Single(_currentPlayers[0], RpcTargetUse.Temp));
            }
        }

        [Rpc(target: SendTo.SpecifiedInParams)]
        public void YouLoseRpc(RpcParams rpcParams = default)
        {
            myWinText.text = "You Lose! /n better luck next time";
            myWinText.gameObject.SetActive(true);
        }
        [Rpc(target: SendTo.SpecifiedInParams)]
        public void YouWinRpc(RpcParams rpcParams)
        {
            myWinText.text = "You Win! /n You are awesome";
            myWinText.gameObject.SetActive(true);
        }
    }
}
