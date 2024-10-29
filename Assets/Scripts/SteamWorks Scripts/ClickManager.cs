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
    [SerializeField] private float timeForNextSkin;
    public NetworkVariable<float> TimeLeft = new NetworkVariable<float>();
    [SerializeField] private TMP_Text timerText;

    [Header("Refs")]
    [SerializeField] private SkinsManager skinManager;

    private void Start()
    {
        TimeLeft.OnValueChanged += UpdateTimer;
        ClickCount.OnValueChanged += UpdateClickAmount;

        if (IsHost)
        {
            timeForNextSkin = defaultTimeForNextSkin / LobbySaver.instance.currentLobby.Value.MemberCount;

            if (LobbySaver.instance.hasDataSaved)
            {
                LoadData();
            }
            else
            {
                TimeLeft.Value = timeForNextSkin;
            }
            SteamMatchmaking.OnLobbyMemberLeave += RecalculateTime;
            SteamMatchmaking.OnLobbyMemberJoined += RecalculateTime;
        }

        canGetSkin = percentOfTimeLeft >= timePercentToGetSkin;
        ToggleCanGetSkinText();
    }

    private void ToggleCanGetSkinText()
    => canGetSkinText.SetActive(!canGetSkin);

    private void LoadData()
    {
        TimeLeft.Value = LobbySaver.instance.percentOfTimeLeftBeforeChange * timeForNextSkin;
        ClickCount.Value = LobbySaver.instance.clicksBeforeChange;
        clickCountText.text = ClickCount.ToString();
    }

    private void Update()
    {
        TimeLeft.Value -= Time.deltaTime;
        if(TimeLeft.Value < 0) TimeLeft.Value = 0;
    }

    private void UpdateTimer(float _prev, float _new)
        => timerText.text = Mathf.Floor(_new / 60) + ":" + Mathf.Floor(_new % 60);

    public void OnClick()
    {
        UpdateClickCount();
        ResetTimerServerRpc();
    }

    private void UpdateClickCount()
    {
        ClickCount.Value++;
    }

    private void UpdateClickAmount(int _prev, int _new)
    {
        clickCountText.text = _new.ToString();

        skinManager.ClickAnim();
    }


    [Rpc(SendTo.Everyone)]
    private void ResetTimerServerRpc()
    {
        if (TimeLeft.Value <= 0)
        {
            TimeLeft.Value = timeForNextSkin;
            if (IsHost)
            {
                UnlockRpc();
            }
            canGetSkin = true;
            ToggleCanGetSkinText();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void UnlockRpc()
    {
        if (canGetSkin) skinManager.GrantItem(31);
    }

    private void RecalculateTime(Lobby _lobby, Friend _friend)
    {
        timeForNextSkin = defaultTimeForNextSkin / LobbySaver.instance.currentLobby.Value.MemberCount;
        TimeLeft.Value = percentOfTimeLeft * timeForNextSkin;
    }

    public float percentOfTimeLeft
        => TimeLeft.Value / timeForNextSkin;

    public int clicksAmount
        => ClickCount.Value;
}
