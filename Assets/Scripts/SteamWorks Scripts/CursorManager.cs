using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class CursorManager : NetworkBehaviour
{
    [SerializeField] private GameObject CursorPrefab;
    [SerializeField] private Transform canvas;
    private GameObject myCursor;
    private void Start()
    {
        //DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
    }

    private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        SpawnCursors();
    }

    [Rpc(SendTo.Everyone)]
    public void ParentToCanvasRpc(NetworkObjectReference cursorReference)
    {
        if (cursorReference.TryGet(out NetworkObject cursor))
        {
            cursor.transform.SetParent(canvas);
        }
    }

    public void SpawnCursors()
    {
        if (IsHost)
        {
            foreach (KeyValuePair<ulong, NetworkClient> pair in NetworkManager.Singleton.ConnectedClients)
            {
                GameObject cursor = Instantiate(CursorPrefab);
                cursor.GetComponent<NetworkObject>().SpawnAsPlayerObject(pair.Key, true);

                var cursorNetworkObject = cursor.GetComponent<NetworkObject>();
                ParentToCanvasRpc(cursorNetworkObject);
            }
        }
    }
}
