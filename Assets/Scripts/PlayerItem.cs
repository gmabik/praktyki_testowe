using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Steamworks.Data;

public class PlayerItem : MonoBehaviour
{
    // player item for grid of players on lobby screen

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private UnityEngine.UI.Image buttonImage;
    [SerializeField] private UnityEngine.Color color;

    public void SetPlayerInfo(string _name)
    {
        playerNameText.text = _name;
    }

    public void SetColor()
    {
        buttonImage.color = color;
    }
}
