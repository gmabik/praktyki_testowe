using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Steamworks;
using Steamworks.Data;

public class CursorManager : NetworkBehaviour
{
    [SerializeField] private GameObject CursorPrefab;
    [SerializeField] private Transform canvas;

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnCursorForJoinedPlayer;
    }

    private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        SpawnCursorsForEachPlayer();
    }

    [Rpc(SendTo.Everyone)]
    public void ParentToCanvasRpc(NetworkObjectReference cursorReference)
    {
        if (cursorReference.TryGet(out NetworkObject cursor))
        {
            cursor.transform.SetParent(canvas);
        }
    }

    private void SpawnCursor(ulong id)
    {
        GameObject cursor = Instantiate(CursorPrefab);
        cursor.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);

        var cursorNetworkObject = cursor.GetComponent<NetworkObject>();
        ParentToCanvasRpc(cursorNetworkObject);
    }

    public void SpawnCursorsForEachPlayer()
    {
        if (IsHost)
        {
            foreach (KeyValuePair<ulong, NetworkClient> pair in NetworkManager.Singleton.ConnectedClients)
            {
                SpawnCursor(pair.Key);
            }
        }
    }

    private void SpawnCursorForJoinedPlayer(ulong id)
    {
        if (IsHost) SpawnCursor(id);
    }
}
