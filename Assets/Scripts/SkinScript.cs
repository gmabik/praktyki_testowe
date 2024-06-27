using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SkinScript : MonoBehaviour
{
    public bool isUnlocked { get; private set; }
    private Image image;
    public ClickManager manager;

    private void Start()
    {
        image = GetComponent<Image>();
        isUnlocked = false;
        image.color = Color.black;
    }

    [PunRPC]
    public void Unlock()
    {
        isUnlocked = true;
        image.color = Color.white;
    }

    public void OnClick()
    {
        if(isUnlocked) GetComponent<PhotonView>().RPC("SetSkin", RpcTarget.All);
    }

    [PunRPC]
    private void SetSkin()
    {
        manager.clickButton.GetComponent<Image>().sprite = image.sprite;
    }
}
