using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostChangeStarter : MonoBehaviour
{
    [SerializeField] private ulong hostID;
    private void OnEnable()
    {
        SteamMatchmaking.OnLobbyMemberLeave += LobbyMemberLeft;
        StartCoroutine(DEBUG_printOwnerName());
        hostID = LobbySaver.instance.currentLobby.Value.Owner.Id.Value;
    }

    private void OnDisable()
    {
        SteamMatchmaking.OnLobbyMemberLeave -= LobbyMemberLeft;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LobbySaver.instance.SaveData(gameObject.GetComponent<ClickManager>().clicksAmount, gameObject.GetComponent<ClickManager>().percentOfTimeLeft);
            SceneManager.LoadScene("HostChangeScreen");
        }
    }

    private void LobbyMemberLeft(Lobby lobby, Friend friend)
    {
        if (hostID == friend.Id)
        {
            LobbySaver.instance.SaveData(gameObject.GetComponent<ClickManager>().clicksAmount, gameObject.GetComponent<ClickManager>().percentOfTimeLeft);
            SceneManager.LoadScene("HostChangeScreen");
        }
    }

    private IEnumerator DEBUG_printOwnerName()
    {
        while (true)
        {
            Debug.Log("Current host: " + LobbySaver.instance.currentLobby.Value.Owner.Name);
            yield return new WaitForSeconds(5f);
        }
    }

    /*private void SaveDataBeforeChange()
    {
        LobbySaver.instance.clicksBeforeChange = gameObject.GetComponent<ClickManager>().clicksAmount;
        LobbySaver.instance.percentOfTimeLeftBeforeChange = gameObject.GetComponent<ClickManager>().percentOfTimeLeft;
    }*/
}
