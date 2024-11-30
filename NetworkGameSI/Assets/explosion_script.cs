using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.Player;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.obj
{
    public class explosion_script : NetworkBehaviour
    {
        public float _damage = 40;

        // Non-RPC method for the AnimationEvent
        public void EndAnim()
        {
            // Ensure the RPC is only called by the server
            if (IsServer)
            {
                EndAnimRpc();
            }
        }

        // RPC to despawn the object
        [Rpc(target: SendTo.Server)]
        private void EndAnimRpc()
        {
            NetworkObject.Despawn();
        }

        private void OnCollisionEnter(Collision other)
{
    Debug.Log($"Collision detected with: {other.gameObject.name}");
    if (other.gameObject.CompareTag("Player"))
    {
        Debug.Log("Player hit!");
        var healthScript = other.gameObject.GetComponent<Health_Net_Prefab>();
        if (healthScript != null)
        {
            Debug.Log("Found Health_Net_Prefab component");
            healthScript.DamageObjectRpc(_damage);
        }
    }
}


    }
}
