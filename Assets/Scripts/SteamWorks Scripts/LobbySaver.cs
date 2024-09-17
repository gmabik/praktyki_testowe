using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySaver : MonoBehaviour
{
    public static LobbySaver instance;
    public Lobby? currentLobby;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
