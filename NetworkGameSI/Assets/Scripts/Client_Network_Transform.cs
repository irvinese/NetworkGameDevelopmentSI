using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

[DisallowMultipleComponent]
public class Client_Network_Transform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
            return false;
    }
}
