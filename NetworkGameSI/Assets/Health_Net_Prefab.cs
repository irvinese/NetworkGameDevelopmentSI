using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using App.Resource.Scripts;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace App.Resource.Scripts.Player{
public class Health_Net_Prefab : NetworkBehaviour
{
    [SerializeField] public float _startingHealth = 100f;
    [SerializeField] public float cooldown = 1.5f;
    [SerializeField] private bool _canDamage = true;
    [SerializeField] private NetworkVariable<float> _Health = new NetworkVariable<float>(100);
    public Image _healthBar;

    private Game_Scripts _gameScript;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _Health.Value = _startingHealth;

        _Health.OnValueChanged += UpdateHealth;
    }

    private void UpdateHealth(float previousvalue, float newvalue)
    {
        if (_healthBar != null)
        {
            _healthBar.fillAmount = newvalue / _startingHealth;
        }

        if (IsOwner)
        {
            if (newvalue < 0f)
            {
                FindObjectOfType<Game_Scripts>().PlayerDeathRpc();
                HasDiedRpc();
            }
        }
        //TODO: sync network data here
    }

    [Rpc(target: SendTo.Server)]
    public void HasDiedRpc()
    {
        NetworkObject.Despawn();
    }

    [Rpc(target: SendTo.Server, RequireOwnership = false)]
public void DamageObjectRpc(float dmg) // Renamed to DamageObjectRpc
{
    if (!_canDamage) return;
    _Health.Value -= dmg;
    StartCoroutine(nameof(DamageCooldown));
    Debug.Log(message:$"Damage received : {dmg}");
}


    private IEnumerable DamageCooldown()
    {
        _canDamage = false;
        yield return new WaitForSeconds(cooldown);
        _canDamage = true;
    }
    }
}
