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
                print("init");
            }
            print(GetName());
            StartCoroutine(CallBacks());
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
