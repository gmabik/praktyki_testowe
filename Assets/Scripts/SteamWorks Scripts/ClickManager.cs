using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class ClickManager : NetworkBehaviour
{
    [SerializeField] private int clickCount;
    [SerializeField] private TMP_Text clickCountText;
    [SerializeField] private float timeForNextSkin;
    [SerializeField] private float timeLeft;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private List<SkinScript> skins;
    public GameObject clickButton;

    private void Start()
    {
        timeForNextSkin /= NetworkManager.Singleton.ConnectedClients.Count;
        timeLeft = timeForNextSkin;
        foreach(SkinScript script in skins)
        {
            script.manager = this;
        }
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

    [ServerRpc]
    private void UpdateClickCountServerRpc()
    {
        clickCount++;
        clickCountText.text = clickCount.ToString();
    }

    [ServerRpc]
    private void ResetTimerServerRpc()
    {
        if (timeLeft <= 0)
        {
            timeLeft = timeForNextSkin;
            int randomSkinNum = 0;
            if (IsHost)
            {
                randomSkinNum = Random.Range(0, skins.Count);
                UnlockSkinServerRpc(randomSkinNum);
            }
        }
    }

    [ServerRpc]
    private void UnlockSkinServerRpc(int random)
    {
        print(random);
        skins[random].Unlock();
    }
}
