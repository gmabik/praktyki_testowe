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

    public void ParentToCanvas(NetworkObjectReference cursorReference)
    {
        if (cursorReference.TryGet(out NetworkObject cursor))
        {
            cursor.transform.SetParent(canvas);
        }
    }

    private void SpawnCursor(ulong id)
    {
        GameObject cursor = Instantiate(CursorPrefab, canvas);
        cursor.GetComponent<CursorScript>().cursorManager = this;
        cursor.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);

        var cursorNetworkObject = cursor.GetComponent<NetworkObject>();
        ParentToCanvas(cursorNetworkObject);
        RescaleCursorRpc(cursorNetworkObject);
    }

    [Rpc(SendTo.Everyone)]
    private void RescaleCursorRpc(NetworkObjectReference cursorReference)
    {
        if (cursorReference.TryGet(out NetworkObject cursor))
        {
            cursor.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        }
    }

    public void SpawnCursorsForEachPlayer()
    {
        if (IsServer)
        {
            foreach (KeyValuePair<ulong, NetworkClient> pair in NetworkManager.Singleton.ConnectedClients)
            {
                SpawnCursor(pair.Key);
            }
        }
    }

    private void SpawnCursorForJoinedPlayer(ulong id)
    {
        if (IsServer)
        {
            SpawnCursor(id);
            foreach (CursorScript script in cursorScripts) 
            { 
                if(script != null) script.SetDataForOtherClientsRpc(); 
            }
        }
    }

    private void AddCursorToList(CursorScript cursorScript)
    {
        cursorScripts.Add(cursorScript);
    }

    public Action<CursorScript> OnCursorSpawnComplete;
}
