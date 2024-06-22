using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_Text playerAmount;
    private LobbyManager lobbyManager;

    public void SetRoomInfo(string _roomName, int _playersAmount, int _maxPlayers, LobbyManager manager)
    {
        roomName.text = _roomName;
        playerAmount.text = _playersAmount.ToString() + "/" + _maxPlayers.ToString();
        lobbyManager = manager;
    }

    public void OnClickItem()
    {

    }
}
