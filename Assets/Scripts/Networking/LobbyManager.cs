using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Panels")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject currentRoomPanel;
    [Header("Lobby")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_Text createButtonText;
    [SerializeField] private Transform roomsContent;
    [SerializeField] private RoomItem roomPrefab;
    private List<RoomItem> roomItems = new List<RoomItem>();
    [SerializeField] private float updateCooldown;
    [Header("CurrentRoom")]
    [SerializeField] private TMP_Text currentRoomName;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    private void Start()
    {
        PhotonNetwork.JoinLobby();
        lobbyPanel.SetActive(true);
        currentRoomPanel.SetActive(false);
    }

    public void OnClickCreateRoom()
    {
        if (roomNameInput.text.Length >= 3)
        {
            PhotonNetwork.CreateRoom(roomNameInput.text, new RoomOptions() { MaxPlayers = 4 });
        }
        else StartCoroutine(WrongName());
    }

    private IEnumerator WrongName()
    {
        createButtonText.text = "Wrong name";
        yield return new WaitForSeconds(1f);
        createButtonText.text = "Create";
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        currentRoomPanel.SetActive(true);
        currentRoomName.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
    }

    float nextUpdateTime;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time > nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + updateCooldown;
        }
    }

    private void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItems) Destroy(item.gameObject);
        roomItems.Clear();

        foreach (RoomInfo roomInfo in list)
        {
            RoomItem newRoom = Instantiate(roomPrefab, roomsContent);
            newRoom.SetRoomInfo(roomInfo.Name, roomInfo.PlayerCount, roomInfo.MaxPlayers, this);
            roomItems.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        lobbyPanel.SetActive(true);
        currentRoomPanel.SetActive(false);
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        SceneManager.LoadScene("ConnectToServer");
    }
}
