using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using DG.Tweening;
using Steamworks.Data;
using Steamworks;
using System;

public class ClickManager : NetworkBehaviour
{
    [SerializeField] private int clickCount;
    [SerializeField] private TMP_Text clickCountText;
    [SerializeField] private float defaultTimeForNextSkin;
    [SerializeField] private float timeForNextSkin;
    [SerializeField] private float timeLeft;
    [SerializeField] private TMP_Text timerText;
    public GameObject clickButton;
    [SerializeField]
    private SkinsManager skinManager;

    private void Start()
    {
        timeForNextSkin = defaultTimeForNextSkin / LobbySaver.instance.currentLobby.Value.MemberCount;
        timeLeft = timeForNextSkin;
        SteamMatchmaking.OnLobbyMemberLeave += RecalculateTime;
        SteamMatchmaking.OnLobbyMemberJoined += RecalculateTime;
    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;
        if(timeLeft < 0) timeLeft = 0;
        timerText.text = Mathf.Floor(timeLeft / 60) + ":" + Mathf.Floor(timeLeft % 60);
    }

    public void OnClick()
    {
        UpdateClickCountServerRpc();
        ResetTimerServerRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateClickCountServerRpc()
    {
        clickCount++;
        clickCountText.text = clickCount.ToString();

        skinManager.ClickAnim();
    }

    [Rpc(SendTo.Everyone)]
    private void ResetTimerServerRpc()
    {
        if (timeLeft <= 0)
        {
            timeLeft = timeForNextSkin;
            //int randomMatNum = 0;
            if (IsHost)
            {
                //randomMatNum = Random.Range(0, skinManager.matButtons.Count * 10);
                UnlockRpc(/*randomMatNum % skinManager.matButtons.Count*/);
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void UnlockRpc()
    {
        //skinManager.matButtons[num].GetComponent<MaterialScript>().Unlock();
        skinManager.GrantItem(31);
    }

    private void RecalculateTime(Lobby _lobby, Friend _friend)
    {
        float percent = timeLeft / timeForNextSkin;
        timeForNextSkin = defaultTimeForNextSkin / LobbySaver.instance.currentLobby.Value.MemberCount;
        timeLeft = percent * timeForNextSkin;
    }
}
