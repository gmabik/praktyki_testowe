using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.SceneManagement;

public class SteamInit : MonoBehaviour
{
    private bool isConnected = false;
    private void Start()
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
    }

    private string GetName()
        => SteamClient.Name;

    private IEnumerator CallBacks()
    {
        while (true)
        {
            SteamClient.RunCallbacks();
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnClick()
    {
        if(isConnected) SceneManager.LoadScene("LobbyMenu");
    }

    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }
}
