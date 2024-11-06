using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Collections;

public struct PlayerInfoData : INetworkSerializable, IEquatable<PlayerInfoData>
{
    public ulong _clientId;
    public FixedString64Bytes _name;
    public bool _isPlayerReady;
    public Color _colorId;

    public PlayerInfoData(ulong Id)
    {
        _clientId = Id;
        _name = (FixedString64Bytes)"";
        _isPlayerReady = false;
        _colorId = Color.magenta;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
{
    serializer.SerializeValue(ref _clientId);
    serializer.SerializeValue(ref _name);
    serializer.SerializeValue(ref _isPlayerReady);

    byte r = (byte)(_colorId.r * 255);
    byte g = (byte)(_colorId.g * 255);
    byte b = (byte)(_colorId.b * 255);
    byte a = (byte)(_colorId.a * 255);
    serializer.SerializeValue(ref r);
    serializer.SerializeValue(ref g);
    serializer.SerializeValue(ref b);
    serializer.SerializeValue(ref a);

    if (serializer.IsReader)
    {
        _colorId = new Color32(r, g, b, a);
    }
}


    public bool Equals(PlayerInfoData other)
    {
        return _clientId == other._clientId && _name.Equals(other._name) && _isPlayerReady == other._isPlayerReady && _colorId.Equals(other._colorId);
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerInfoData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_clientId, _name, _isPlayerReady, _colorId);
    }

   public override string ToString() => _name.Value.ToString();

   public static implicit operator string(PlayerInfoData name)=> name.ToString();

   public static implicit operator PlayerInfoData(string s) => new PlayerInfoData{_name = new FixedString64Bytes(s)};
}


