using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace App.Resource.Scripts
{
public class Projectile_Object : NetworkBehaviour
{
    [SerializeField] float _speed = 40f;
    [SerializeField] private float _damage = 10;
    [SerializeField] private float destructTime = 5f;

    //TODO hook up rojectiles to pool system

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponent<Rigidbody>().velocity = this.transform.forward * _speed;
        StartCoroutine(routine: AutoDestruct());
    }

    private void OnCollisionEnter(Collision other){
        //make sure its a player and that the bullet dosen't match the owner, meaning we can't do friendly fire.
        if (other.gameObject.tag.Equals("Player") && other.gameObject.GetComponent<NetworkObject>().OwnerClientId != this.OwnerClientId)
        {
            other.gameObject.GetComponent<Health_Net_Prefab>().DamageObjectRpc(_damage);
        }
    }

    private IEnumerator AutoDestruct()
    {
        yield return new WaitForSeconds(destructTime);
        this.NetworkObject.Despawn();
    }
}
}
