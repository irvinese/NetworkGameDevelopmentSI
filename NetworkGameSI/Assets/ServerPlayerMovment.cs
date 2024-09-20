using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovment : NetworkBehaviour
{
   [SerializeField] public float _pSpeed;
   [SerializeField] private Transform _pTransform;
   [SerializeField] private float _pGravity = -2;
   private Vector3 _velocity;

    public CharacterController _CC;
    private PlayerInputAction _playerInput;

    // Start is called before the first frame update
    void Start()
    {
        _playerInput = new PlayerInputAction();
        _playerInput.Enable();
        
    }

    // Update is called once per frame
    void Update()
    {

        if(!IsOwner) return;

        Vector2 moveInput = _playerInput.Player.Move.ReadValue<Vector2>();

        if (IsServer)
        {
            Movement(moveInput);
        }else if (IsClient)
        {
            MoveServerRPC(moveInput);
        }



        
    }

    private void Movement(Vector2 _input)
{
    
    Vector3 _moveDirection = (_input.x * _pTransform.right + _input.y * _pTransform.forward);
    
    _CC.Move(_moveDirection * _pSpeed * Time.deltaTime);

    //extra code to add gravity
    if(!_CC.isGrounded)
    {
        _velocity.y += _pGravity * Time.deltaTime;
    }else
    {
        _velocity.y = -2f;
    }

    _CC.Move(_velocity * Time.deltaTime);
}


    [Rpc(target:SendTo.Server)]
    private void MoveServerRPC(Vector2 _input)
    {
        Movement(_input);
    }
}
