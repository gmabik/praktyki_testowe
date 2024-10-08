using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Steamworks.ServerList;
using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostManager : MonoBehaviour
{
    [SerializeField] private GameObject hostLeftText;

    private void Start()
    {
        StartCoroutine(HostLeft());
    }

    private IEnumerator HostLeft()
    {
        Debug.LogError(LobbySaver.instance.currentLobby.Value.Owner.Name + "         " + LobbySaver.instance.currentLobby.Value.Owner.Id == SteamClient.SteamId + "");

        hostLeftText.GetComponent<TMP_Text>().text = "Previous host left. Changing host to " + LobbySaver.instance.currentLobby.Value.Owner.Name + "...";

        NetworkManager.Singleton.Shutdown();

        yield return new WaitForSeconds(1f);

        if (LobbySaver.instance.currentLobby.Value.Owner.Id == SteamClient.SteamId)
        {
            while (!NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.StartHost();
                yield return new WaitForSeconds(1f);
            }

            while (!HasAllConnected())
            {
                yield return new WaitForSeconds(1f);
            }

            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
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
    }

    [Rpc(SendTo.Server)]
    private bool HasAllConnected()
        => NetworkManager.Singleton.ConnectedClients.Count == LobbySaver.instance.currentLobby.Value.MemberCount;
}
