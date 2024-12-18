using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySaver : MonoBehaviour
{
    //saves current lobby data for use and current game data for host transition

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
