using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Steamworks;
using Steamworks.Data;
using System;

public class CursorManager : NetworkBehaviour
{
    [SerializeField] private GameObject CursorPrefab;
    [SerializeField] private List<CursorScript> cursorScripts;
    [SerializeField] private Transform canvas;

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnCursorForJoinedPlayer;
    }

    private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        OnCursorSpawnComplete += AddCursorToList;
        SpawnCursorsForEachPlayer();
    }

    [Rpc(SendTo.Everyone)]
    public void ParentToCanvasRpc(NetworkObjectReference cursorReference)
    {
        if (cursorReference.TryGet(out NetworkObject cursor))
        {
            cursor.transform.SetParent(canvas);
            cursor.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
    }

    private void SpawnCursor(ulong id)
    {
        GameObject cursor = Instantiate(CursorPrefab);
        cursor.GetComponent<CursorScript>().cursorManager = this;
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
        if (IsHost)
        {
            SpawnCursor(id);
            foreach (CursorScript script in cursorScripts) script.SetDataRpc();
        }
    }

    private void AddCursorToList(CursorScript cursorScript)
    {
        if (IsHost) cursorScripts.Add(cursorScript);
    }

    public Action<CursorScript> OnCursorSpawnComplete;
}
