using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
public class Movement : NetworkBehaviour
{
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private OwnerNetworkAnimator _ownerAnimator;
    [SerializeField] private float _jumpHeight = 100.5f;
    [SerializeField] private float _gravity = -9.81f;
    private Vector3 _velocity;
    private CharacterController _characterController;

    private void Awake()
    {
        if (_myAnimator == null)
        {
            _myAnimator = gameObject.GetComponent<Animator>();
        }

        if (_ownerAnimator == null)
        {
            _ownerAnimator = gameObject.GetComponent<OwnerNetworkAnimator>();
        }
    }
  
    private void FixedUpdate()
    {

        if(!IsOwner) return;

        Vector3 moveDirection = new Vector3(0, 0, 0);
        if(Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
        if(Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
        if(Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
        if(Input.GetKey(KeyCode.D)) moveDirection.x = +1f;
        if(Input.GetKey(KeyCode.Space)) _ownerAnimator.SetTrigger("JumpTrigger");
        if(Input.GetKey(KeyCode.Z)) _ownerAnimator.SetTrigger("PunchTrigger");

        // Jump logic
            if (Input.GetKey(KeyCode.Space) && _characterController.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(2 * _jumpHeight * -_gravity);
                _ownerAnimator.SetTrigger("JumpTrigger");
            }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            _myAnimator.SetBool("IsRunning", true);
        }else{
            _myAnimator.SetBool("IsRunning", false);
        }

            _myAnimator.SetBool(name:"IsWalking", moveDirection.z != 0 || moveDirection.x != 0);
        

        float movespeed = .03f;
        transform.position += moveDirection * (movespeed + Time.deltaTime);

        // Apply gravity
            if (!_characterController.isGrounded)
            {
                _velocity.y += _gravity * Time.deltaTime;
            }
            else if (_velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            // Apply the velocity (for jump and gravity)
            _characterController.Move(_velocity * Time.deltaTime);

    }
}

}