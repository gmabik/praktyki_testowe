using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.SceneManagement;

public class SteamInit : MonoBehaviour
{
    private bool isConnected = false;
    private async void Start() //initiates a connection to steam services, loads item definitions for later use
    {
        try
        {
            if (!SteamClient.IsValid)
            {
                SteamClient.Init(3092070);
                print("init " + GetName());
            }
            StartCoroutine(CallBacks());
            DontDestroyOnLoad(gameObject);
            isConnected = true;
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            isConnected = false;
        }
        Debug.LogError("isConnected: " + isConnected);
        await SteamInventory.GetAllItemsAsync();
        SteamInventory.LoadItemDefinitions();
    }

    private string GetName() //gets client's name
        => SteamClient.Name;

    private IEnumerator CallBacks() // w/o this callbacks wouldn't work
    {
        while (true)
        {
            SteamClient.RunCallbacks();
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnClick() // loads lobby connecting scene
    {
        if(isConnected) SceneManager.LoadScene("LobbyMenu");
    }

    private void OnApplicationQuit() // closes connection to steam services when app is closed
    {
        SteamClient.Shutdown();
    }
}
