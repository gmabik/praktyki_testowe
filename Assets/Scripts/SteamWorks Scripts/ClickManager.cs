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
    public int ClickCount;
    [SerializeField] private TMP_Text clickCountText;
    [SerializeField] private float defaultTimeForNextSkin;
    [SerializeField] private float timeForNextSkin;
    public float TimeLeft;
    [SerializeField] private TMP_Text timerText;
    public GameObject clickButton;
    [SerializeField]
    private SkinsManager skinManager;

    private void Start()
    {
        timeForNextSkin = defaultTimeForNextSkin / LobbySaver.instance.currentLobby.Value.MemberCount;
        if (LobbySaver.instance.hasDataSaved)
        {
            LoadData();
        }
        else
        {
            TimeLeft = timeForNextSkin;
        }

        SteamMatchmaking.OnLobbyMemberLeave += RecalculateTime;
        SteamMatchmaking.OnLobbyMemberJoined += RecalculateTime;
    }

    private void LoadData()
    {
        TimeLeft = LobbySaver.instance.percentOfTimeLeftBeforeChange * timeForNextSkin;
        ClickCount = LobbySaver.instance.clicksBeforeChange;
        clickCountText.text = ClickCount.ToString();
    }

    private void Update()
    {
        TimeLeft -= Time.deltaTime;
        if(TimeLeft < 0) TimeLeft = 0;
        timerText.text = Mathf.Floor(TimeLeft / 60) + ":" + Mathf.Floor(TimeLeft % 60);
    }

    public void OnClick()
    {
        UpdateClickCountServerRpc();
        ResetTimerServerRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateClickCountServerRpc()
    {
        ClickCount++;
        clickCountText.text = ClickCount.ToString();

        skinManager.ClickAnim();
    }

    [Rpc(SendTo.Everyone)]
    private void ResetTimerServerRpc()
    {
        if (TimeLeft <= 0)
        {
            TimeLeft = timeForNextSkin;
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
        timeForNextSkin = defaultTimeForNextSkin / LobbySaver.instance.currentLobby.Value.MemberCount;
        TimeLeft = percentOfTimeLeft * timeForNextSkin;
    }

    public float percentOfTimeLeft
        => TimeLeft / timeForNextSkin;

    public int clicksAmount
        => ClickCount;
}
