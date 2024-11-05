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
    [SerializeField] private float timePercentToGetSkin;
    [SerializeField] private bool canGetSkin;
    [SerializeField] private GameObject canGetSkinText;

    [Header("Clicks")]
    public NetworkVariable<int> ClickCount = new NetworkVariable<int>();
    [SerializeField] private TMP_Text clickCountText;
    public GameObject clickButton;

    [Header("Time")]
    [SerializeField] private float defaultTimeForNextSkin;
    public NetworkVariable<float> timeForNextSkin = new NetworkVariable<float>();
    public NetworkVariable<float> TimeLeft = new NetworkVariable<float>();
    [SerializeField] private TMP_Text timerText;

    [Header("Refs")]
    [SerializeField] private SkinsManager skinManager;

    private void Start()
    {
        TimeLeft.OnValueChanged += UpdateTimer;
        ClickCount.OnValueChanged += UpdateClickAmount;
        if (IsServer)
        {
            timeForNextSkin.Value = defaultTimeForNextSkin / LobbySaver.instance.currentLobby.Value.MemberCount;

            if (LobbySaver.instance.hasDataSaved)
            {
                LoadData();
            }
            else
            {
                TimeLeft.Value = timeForNextSkin.Value;
            }
            ManageSkinGettingRpc();
            SteamMatchmaking.OnLobbyMemberLeave += RecalculateTime;
            SteamMatchmaking.OnLobbyMemberJoined += RecalculateTime;
        }
        ManageSkinGetting();
    }

    private void ManageSkinGetting()
    {
        if (timeForNextSkin.Value == 0) return;
        if (!LobbySaver.instance.hasDataSaved) canGetSkin = percentOfTimeLeft >= timePercentToGetSkin;
        else canGetSkin = true;

        ToggleCanGetSkinText();
    }

    [Rpc(SendTo.Everyone)]
    private void ManageSkinGettingRpc()
    {
        if (!LobbySaver.instance.hasDataSaved) canGetSkin = percentOfTimeLeft >= timePercentToGetSkin;
        else canGetSkin = true;

        ToggleCanGetSkinText();
    }

    private void ToggleCanGetSkinText()
        => canGetSkinText.SetActive(!canGetSkin);

    private void LoadData()
    {
        TimeLeft.Value = LobbySaver.instance.percentOfTimeLeftBeforeChange * timeForNextSkin.Value;
        ClickCount.Value = LobbySaver.instance.clicksBeforeChange;
        clickCountText.text = ClickCount.Value.ToString();
    }

    private void Update()
    {
        if (!IsServer) return;
        TimeLeft.Value -= Time.deltaTime;
        if(TimeLeft.Value < 0) TimeLeft.Value = 0;
    }

    private void UpdateTimer(float _prev, float _new)
        => timerText.text = Mathf.Floor(_new / 60) + ":" + Mathf.Floor(_new % 60);

    public void OnClick()
    {
        UpdateClickCountRpc();
        ResetTimerServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void UpdateClickCountRpc()
    {
        ClickCount.Value++;
    }

    private void UpdateClickAmount(int _prev, int _new)
    {
        clickCountText.text = _new.ToString();

        skinManager.ClickAnim();
    }


    [Rpc(SendTo.Server)]
    private void ResetTimerServerRpc()
    {
        if (TimeLeft.Value <= 0)
        {
            TimeLeft.Value = timeForNextSkin.Value;
            UnlockRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void UnlockRpc()
    {
        if (canGetSkin) skinManager.GrantItem(31);
        canGetSkin = true;
        ToggleCanGetSkinText();
    }

    private void RecalculateTime(Lobby _lobby, Friend _friend)
    {
        timeForNextSkin.Value = defaultTimeForNextSkin / LobbySaver.instance.currentLobby.Value.MemberCount;
        TimeLeft.Value = percentOfTimeLeft * timeForNextSkin.Value;
    }

    public float percentOfTimeLeft
        => TimeLeft.Value / timeForNextSkin.Value;

    public int clicksAmount
        => ClickCount.Value;
}
