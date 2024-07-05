using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

public class CursorManager : NetworkBehaviour
{
    [SerializeField] private GameObject CursorPrefab;
    [SerializeField] private Transform canvas;
    private GameObject myCursor;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneLoaded;
    }

    private void SceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if(IsHost && sceneName == "Game")
        {
            foreach(ulong id in clientsCompleted)
            {
                GameObject cursor = Instantiate(CursorPrefab, canvas);
                cursor.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, true);
            }
        }
    }
}
