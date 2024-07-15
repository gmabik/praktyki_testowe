using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using DG.Tweening;

public class ClickManager : NetworkBehaviour
{
    [SerializeField] private int clickCount;
    [SerializeField] private TMP_Text clickCountText;
    [SerializeField] private float timeForNextSkin;
    [SerializeField] private float timeLeft;
    [SerializeField] private TMP_Text timerText;
    public GameObject clickButton;
    [SerializeField]
    private SkinsManager skinManager;

    private void Start()
    {
        timeForNextSkin /= LobbySaver.instance.currentLobby.Value.MemberCount;
        timeLeft = timeForNextSkin;
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
            int randomSkinNum = 0;
            if (IsHost)
            {
                randomSkinNum = Random.Range(0, skinManager.skinButtons.Count * 10);
                skinManager.skinButtons[randomSkinNum % skinManager.skinButtons.Count].GetComponent<SkinScript>().UnlockRpc();
            }
        }
    }
}
