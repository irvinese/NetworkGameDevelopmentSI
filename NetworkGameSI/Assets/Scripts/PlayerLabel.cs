using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerLabel : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerText;
    [SerializeField] private Button _kickButton;
    [SerializeField] private Image _ReadyStatusImg, _PlayerColorImg;

    public event Action<ulong> onKickClicked;
    private ulong _clientId;

    private void OnEnable()
    {
        _kickButton.onClick.AddListener(ButtonKick_clicked);
    }

    private void OnDisable()
    {
        _kickButton.onClick.RemoveListener(ButtonKick_clicked);
    }

    public void SetPlayerLabelName(ulong playerName)
    {
        _clientId = playerName;
        _playerText.text = "Player " + playerName.ToString();
    }

    private void ButtonKick_clicked()
    {
        onKickClicked?.Invoke(_clientId);
    }

    public void SetKickActive(bool isOn)
    {
        _kickButton.gameObject.SetActive(isOn);
    }

    public void SetReady(bool ready)
    {
        _ReadyStatusImg.color = ready ? Color.green : Color.red;
    }

    public void SetPlayerColor(Color color)
    {
        _PlayerColorImg.color = color;
    }
}
