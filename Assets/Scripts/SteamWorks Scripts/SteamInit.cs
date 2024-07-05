using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamInit : MonoBehaviour
{
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
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
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

    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }
}
