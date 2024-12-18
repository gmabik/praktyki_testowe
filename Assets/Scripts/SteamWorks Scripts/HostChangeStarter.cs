using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    // was used for testing by one person
    /*private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LobbySaver.instance.SaveData(gameObject.GetComponent<ClickManager>().clicksAmount, gameObject.GetComponent<ClickManager>().percentOfTimeLeft);
            SceneManager.LoadScene("HostChangeScreen");
        }
    }*/

    private void LobbyMemberLeft(Lobby lobby, Friend friend)
    {
        if (hostID == friend.Id)
        {
            LobbySaver.instance.SaveData(gameObject.GetComponent<ClickManager>().clicksAmount, gameObject.GetComponent<ClickManager>().percentOfTimeLeft);
            SceneManager.LoadScene("HostChangeScreen");
        }
        else
        {
            StartCoroutine(PlayerLeftTextPopUp(friend.Name));
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

    [SerializeField] private TMP_Text playerLeftText;
    private IEnumerator PlayerLeftTextPopUp(string name)
    {
        playerLeftText.text = name + " left the game";
        playerLeftText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        playerLeftText.gameObject.SetActive(false);
    }

    /*private void SaveDataBeforeChange()
    {
        LobbySaver.instance.clicksBeforeChange = gameObject.GetComponent<ClickManager>().clicksAmount;
        LobbySaver.instance.percentOfTimeLeftBeforeChange = gameObject.GetComponent<ClickManager>().percentOfTimeLeft;
    }*/
}
