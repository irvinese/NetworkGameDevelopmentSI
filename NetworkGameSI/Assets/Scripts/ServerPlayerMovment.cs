using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ServerPlayerMovment : NetworkBehaviour
{
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetworkAnimator;
   [SerializeField] public float _pSpeed;
   [SerializeField] private Transform _pTransform;
   [SerializeField] private float _pGravity = -2;
   private Vector3 _velocity;

    public CharacterController _CC;
    private PlayerInputAction _playerInput;

    // Start is called before the first frame update
    void Start()
    {

        if(_myAnimator == null)
        {
            _myAnimator = gameObject.GetComponent<Animator>();
        }

        if(_myNetworkAnimator == null)
        {
            _myNetworkAnimator = gameObject.GetComponent<NetworkAnimator>();
        }

        _playerInput = new PlayerInputAction();
        _playerInput.Enable();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(!IsOwner) return;

        Vector2 moveInput = _playerInput.Player.Move.ReadValue<Vector2>();

        bool IsJumping = _playerInput.Player.Jumping.triggered;
        bool IsPunching = _playerInput.Player.Punching.triggered;
        bool IsRunning = _playerInput.Player.Running.triggered;

        if (IsServer)
        {
            Movement(moveInput, IsJumping, IsPunching, IsRunning);
        }else if (IsClient)
        {
            MoveServerRPC(moveInput, IsJumping, IsPunching, IsRunning);
        }



        
    }

    private void Movement(Vector2 _input, bool IsJumping, bool IsPunching, bool IsRunning)
{
    
    Vector3 _moveDirection = (_input.x * _pTransform.right + _input.y * _pTransform.forward);

    _myAnimator.SetBool("IsWalking", _input.x != 0 || _input.y != 0);
    if(IsJumping){ _myNetworkAnimator.SetTrigger("JumpTrigger");}
    if(IsPunching){ _myNetworkAnimator.SetTrigger("PunchTrigger");}
    _myAnimator.SetBool("IsRunning", IsRunning);
    
    if(IsRunning)
    {
        _CC.Move(_moveDirection * (_pSpeed * 1.3f) * Time.deltaTime);
    }else{
        _CC.Move(_moveDirection * _pSpeed * Time.deltaTime);
    }

    // Handle jumping
    if (IsJumping && _CC.isGrounded)
    {
        _velocity.y = Mathf.Sqrt(2 * -_pGravity * 100.5f);
    }
   

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
    private void MoveServerRPC(Vector2 _input, bool IsJumping, bool IsPunching, bool IsRunning)
    {
        Movement(_input, IsJumping, IsPunching, IsRunning);
    }
}
