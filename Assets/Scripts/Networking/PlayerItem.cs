using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    private Image buttonImage;
    [SerializeField] private Color color;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    public void SetPlayerInfo(Player _player)
    {
        playerNameText.text = _player.NickName;
    }

    public void SetColor()
    {
        buttonImage.color = color;
    }
}
