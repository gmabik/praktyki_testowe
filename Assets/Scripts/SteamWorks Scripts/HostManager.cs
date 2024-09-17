using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Steamworks.ServerList;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HostManager : MonoBehaviour
{
    [SerializeField] private GameObject hostLeftText;
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

    private void LobbyMemberLeft(Lobby lobby, Friend friend)
    {
        if (hostID == friend.Id)
        {
            StopAllCoroutines();
            StartCoroutine(HostLeft());
        }
    }

    private IEnumerator HostLeft()
    {
        Debug.LogError(LobbySaver.instance.currentLobby.Value.Owner.Name + "         " + LobbySaver.instance.currentLobby.Value.Owner.Id == SteamClient.SteamId + "");

        hostLeftText.SetActive(true);

        NetworkManager.Singleton.Shutdown();

        hostID = LobbySaver.instance.currentLobby.Value.Owner.Id;
        if (LobbySaver.instance.currentLobby.Value.Owner.Id == SteamClient.SteamId)
        {
            while (!NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.StartHost();
                yield return new WaitForSeconds(1f);
            }
        }
        else
        {
            while (!NetworkManager.Singleton.IsConnectedClient)
            {
                NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = LobbySaver.instance.currentLobby.Value.Owner.Id;
                NetworkManager.Singleton.StartClient();
                yield return new WaitForSeconds(1f);
            }
        }
        hostLeftText.SetActive(false);

        //NetworkManager.Singleton.SceneManager.LoadScene("LobbyMenu", LoadSceneMode.Single);






        /*yield return new WaitForSeconds(5f);
if (LobbySaver.instance.currentLobby.Value.Owner.Id == SteamClient.SteamId)
{
    NetworkManager.Singleton.StartHost();
}
if (NetworkManager.Singleton.IsHost) yield break;
yield return new WaitForSeconds(5f);
NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = LobbySaver.instance.currentLobby.Value.Owner.Id;
NetworkManager.Singleton.StartClient();
        print("is host: " + NetworkManager.Singleton.IsHost);
        if (NetworkManager.Singleton.IsHost && SceneManager.GetActiveScene().name == "LobbyMenu") startGameButton.SetActive(true);*/
    }

    private IEnumerator DEBUG_printOwnerName()
    {
        while (true)
        {
            //if (!LobbySaver.instance.currentLobby.HasValue) yield return new WaitForSeconds(1f);
            Debug.LogError("Steam lobby owner: " + LobbySaver.instance.currentLobby.Value.Owner.Name + ", is Netcode lobby owner: " + NetworkManager.Singleton.IsHost);
            yield return new WaitForSeconds(1f);
        }
    }
}
