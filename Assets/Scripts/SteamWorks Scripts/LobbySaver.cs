using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySaver : MonoBehaviour
{
    public static LobbySaver instance;
    public Lobby? currentLobby;

    public int clicksBeforeChange;
    public float percentOfTimeLeftBeforeChange;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool hasDataSaved
        => clicksBeforeChange != 0 || percentOfTimeLeftBeforeChange != 0;

    public void SaveData(int clicks, float time)
    {
        clicksBeforeChange = clicks;
        percentOfTimeLeftBeforeChange = time;
    }
}
