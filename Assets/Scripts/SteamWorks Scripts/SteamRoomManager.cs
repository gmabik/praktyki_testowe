using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Steamworks;
using Steamworks.Data;

public class SteamRoomManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputLobbyID;
    [SerializeField] private TMP_Text lobbyIDText;

    private void OnEnable()
    {
        SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
        /*SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
        
        SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;*/
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;
        SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
        /*SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
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
        UpdatePlayerList();
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
    */

    private void SteamMatchmaking_OnLobbyMemberLeave(Lobby _lobby, Friend _friend)
    {
        UpdatePlayerList();
    }

    private void SteamMatchmaking_OnLobbyMemberJoined(Lobby _lobby, Friend _friend)
    {
        UpdatePlayerList();
    }



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

    /*public async void JoinLobbyWithID(ulong id)
    {
        Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();
        foreach (Lobby lobby in lobbies)
        {
            if (lobby.Id == id)
            {
                await lobby.Join();
                return;
            }
        }
    }*/

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
        lobbyIDText.text = "ID: " + LobbySaver.instance.currentLobby?.Id.ToString();
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public void LeaveLobby()
    {
        LobbySaver.instance.currentLobby?.Leave();
        LobbySaver.instance.currentLobby = null;

        for (int i = 0; i < playerItemGrid.childCount; i++)
        {
            Destroy(playerItemGrid.GetChild(i).gameObject);
        }

        menuPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    [SerializeField] private PlayerItem playerItemPrefab;
    [SerializeField] private Transform playerItemGrid;
    [SerializeField] private GameObject playButton;
    private void UpdatePlayerList()
    {
        foreach (Friend friend in LobbySaver.instance.currentLobby?.Members)
        {
            PlayerItem playerItem = Instantiate(playerItemPrefab, playerItemGrid);
            playerItem.SetPlayerInfo(friend.Name);

            if (LobbySaver.instance.currentLobby?.Owner.Id == friend.Id)
            {
                playerItem.SetColor();
                playButton.SetActive(true);
            }
        }
    }

    public void CopyID()
    {
        TextEditor editor = new TextEditor();
        editor.text = LobbySaver.instance.currentLobby?.Id.ToString();
        editor.SelectAll();
        editor.Copy();
    }

    /*[SerializeField] private RoomItem lobbyItemPrefab;
    [SerializeField] private Transform lobbyItemGrid;
    private async IAsyncEnumerator<WaitForSeconds> UpdateLobbyList()
    {
        while (true)
        {
            if (LobbySaver.instance.currentLobby == null)
            {
                Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();
                foreach (Lobby lobby in lobbies)
                {
                    RoomItem newLobby = Instantiate(lobbyItemPrefab, lobbyItemGrid);
                    newLobby.SetRoomInfo(lobby.Id, lobby.MemberCount, lobby.MaxMembers, this);
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }*/
}
