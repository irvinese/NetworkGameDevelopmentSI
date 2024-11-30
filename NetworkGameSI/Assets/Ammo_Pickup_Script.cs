using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace App.Resource.Scripts.obj
{
public class Ammo_Pickup_Script : NetworkBehaviour
{
   private void OnCollisionEnter(Collision other)
   {
        if (!IsServer) return;
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Bullet_Spawner>()._ammo.Value++;

            Destroy(gameObject);    
        }
   }
}
}
