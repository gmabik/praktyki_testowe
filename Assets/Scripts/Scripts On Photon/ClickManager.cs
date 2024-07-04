using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ClickManager : MonoBehaviour
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
        timeForNextSkin /= PhotonNetwork.PlayerList.Length;
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
        GetComponent<PhotonView>().RPC("UpdateClickCount", RpcTarget.All);
        GetComponent<PhotonView>().RPC("ResetTimer", RpcTarget.All);
    }

    [PunRPC]
    private void UpdateClickCount()
    {
        clickCount++;
        clickCountText.text = clickCount.ToString();
    }

    [PunRPC]
    private void ResetTimer()
    {
        if (timeLeft <= 0)
        {
            timeLeft = timeForNextSkin;
            int randomSkinNum = 0;
            if (PhotonNetwork.IsMasterClient)
            {
                randomSkinNum = Random.Range(0, skins.Count);
                GetComponent<PhotonView>().RPC("UnlockSkin", RpcTarget.All, randomSkinNum);
            }
        }
    }

    [PunRPC]
    private void UnlockSkin(int random)
    {
        skins[random].Unlock();
    }
}
