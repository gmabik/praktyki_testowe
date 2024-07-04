using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Steamworks;
using Steamworks.Data;

public class SteamRoomManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputLobbyID;

    private void OnEnable()
    {
        SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;
        /*SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;*/
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;
        /*SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;*/
    }

    private async void SteamFriends_OnGameLobbyJoinRequested(Lobby _lobby, SteamId _steamID)
    {
        RoomEnter joinedLobby = await _lobby.Join();
        if (joinedLobby == RoomEnter.Success)
        {
            LobbySaver.instance.currentLobby = _lobby;
            Debug.Log("Joined lobby");
        }
        else Debug.Log("Failed to join lobby");
    }

    private void SteamMatchmaking_OnLobbyCreated(Result _result, Lobby _lobby)
    {
        if (_result != Result.OK)
        {
            Debug.Log("lobby was not created");
            return;
        }
        _lobby.SetPublic();
        _lobby.SetJoinable(true);
    }
    private void SteamMatchmaking_OnLobbyEntered(Lobby _lobby)
    {
        LobbySaver.instance.currentLobby = _lobby;
        OpenLobbyPanel();
    }

    /*
     
    private void SteamMatchmaking_OnLobbyGameCreated(Lobby _lobby, uint _ip, ushort _port, SteamId _steamID)
    {
        Debug.Log("Lobby was created");
    }

    private void SteamMatchmaking_OnLobbyInvite(Friend _friend, Lobby _lobby)
    {
        Debug.Log($"Invite from {_friend.Name}");
    }

    private void SteamMatchmaking_OnLobbyMemberLeave(Lobby _lobby, Friend _friend)
    {
        Debug.Log("member leave");
    }

    private void SteamMatchmaking_OnLobbyMemberJoined(Lobby _lobby, Friend _friend)
    {
        Debug.Log("member join");
    }

    */

    public async void HostLobby()
    {
        await SteamMatchmaking.CreateLobbyAsync(4);
    }

    public async void JoinLobbyWithID()
    {
        if (!ulong.TryParse(inputLobbyID.text, out ulong id)) return;

        Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();
        foreach (Lobby lobby in lobbies) 
        {
            if (lobby.Id == id)
            {
                await lobby.Join();
                return;
            }
        }
    }

    [Header("UI")]
    [SerializeField] private GameObject lobbyCreateOrJoinPanel;
    [SerializeField] private GameObject lobbyJoiningPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject menuPanel;

    public void OpenJoinRoomPanel()
    {
        lobbyCreateOrJoinPanel.SetActive(false);
        lobbyJoiningPanel.SetActive(true);
    }

    public void OpenChoicePanel()
    {
        lobbyCreateOrJoinPanel.SetActive(true);
        lobbyJoiningPanel.SetActive(false);
    }

    public void OpenLobbyPanel()
    {
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public void OpenMenuPanel()
    {
        menuPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }
}
