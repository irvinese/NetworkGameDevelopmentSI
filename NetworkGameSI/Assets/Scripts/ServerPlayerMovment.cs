using System.Collections;
using System.Collections.Generic;
using App.Resource.Scripts.obj;
using Assets.Scripts;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class ServerPlayerMovement : NetworkBehaviour
{
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private NetworkAnimator _myNetworkAnimator;
    [SerializeField] public float _pSpeed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _gravityMultiplier = 2f;
    [SerializeField] private Bullet_Spawner _bulletSpawner;
    
    private Rigidbody _rigidbody;
    private PlayerInputAction _playerInput;
    private Vector3 _moveDirection = Vector3.zero;
    private bool _isGrounded;

    private void Start()
    {
        // Fetch references if not set in the Inspector
        if (_myAnimator == null)
        {
            _myAnimator = GetComponent<Animator>();
        }

        if (_myNetworkAnimator == null)
        {
            _myNetworkAnimator = GetComponent<NetworkAnimator>();
        }

        if (_bulletSpawner == null)
        {
            Debug.LogError("BulletSpawner is not assigned.");
            enabled = false;
            return;
        }

        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody not found on the GameObject.");
            enabled = false;
            return;
        }

        // Enable player input
        _playerInput = new PlayerInputAction();
        _playerInput.Enable();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        Vector2 moveInput = _playerInput.Player.Move.ReadValue<Vector2>();

        bool isJumping = _playerInput.Player.Jumping.triggered;
        bool isPunching = _playerInput.Player.Punching.triggered;
        bool isRunning = _playerInput.Player.Running.IsPressed();

        if (IsServer)
        {
            Movement(moveInput, isJumping, isPunching, isRunning);
        }
        else if (IsClient)
        {
            MoveServerRPC(moveInput, isJumping, isPunching, isRunning);
        }

        if (isPunching)
        {
            _bulletSpawner.FireProjectileRpc();
        }
    }

    private void Movement(Vector2 input, bool isJumping, bool isPunching, bool isRunning)
    {
        // Calculate movement direction
        _moveDirection = new Vector3(input.x, 0f, input.y).normalized;

        // Set animation states
        _myAnimator.SetBool("IsWalking", _moveDirection.magnitude > 0);
        _myAnimator.SetBool("IsRunning", isRunning);

        if (isJumping)
        {
            _myNetworkAnimator.SetTrigger("JumpTrigger");
        }

        if (isPunching)
        {
            _myNetworkAnimator.SetTrigger("PunchTrigger");
        }

        // Update facing direction if moving
        if (_moveDirection != Vector3.zero)
        {
            transform.forward = _moveDirection;
        }

        // Apply movement
        float speed = isRunning ? _pSpeed * 1.3f : _pSpeed;
        Vector3 velocity = _moveDirection * speed;
        velocity.y = _rigidbody.velocity.y; // Preserve vertical velocity
        _rigidbody.velocity = velocity;

        // Handle jumping
        if (isJumping && _isGrounded)
        {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isGrounded = false; // Temporarily disable grounded state
        }

        // Simulate gravity
        if (!_isGrounded)
        {
            _rigidbody.AddForce(Vector3.down * _gravityMultiplier, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Re-enable grounded state when colliding with ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }

    [Rpc(target: SendTo.Server)]
    private void MoveServerRPC(Vector2 input, bool isJumping, bool isPunching, bool isRunning)
    {
        Movement(input, isJumping, isPunching, isRunning);
    }
}
