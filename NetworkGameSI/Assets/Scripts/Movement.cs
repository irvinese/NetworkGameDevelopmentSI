using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
public class Movement : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        Vector3 moveDirection = new Vector3(0, 0, 0);
        if(Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
        if(Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
        if(Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
        if(Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

        float movespeed = .03f;
        transform.position += moveDirection * (movespeed + Time.deltaTime);
    }
}

}