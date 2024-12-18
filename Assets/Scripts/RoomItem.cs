using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomItem : MonoBehaviour
{
    // not used, can be deleted

    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_Text playerAmount;
    private SteamRoomManager manager;
    private ulong id;

    public void SetRoomInfo(ulong _id, int _playersAmount, int _maxPlayers, SteamRoomManager _manager)
    {
        id = _id;
        roomName.text = id.ToString();
        playerAmount.text = _playersAmount.ToString() + "/" + _maxPlayers.ToString();
        manager = _manager;
    }

    public void OnClickItem()
    {
        //manager.JoinLobbyWithID(id);
    }
}
