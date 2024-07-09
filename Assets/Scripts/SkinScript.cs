using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

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

    [Rpc(SendTo.Everyone)]
    public void UnlockRpc()
    {
        isUnlocked = true;
        image.color = Color.white;
    }

    public void OnClick()
    {
        if(isUnlocked) SetSkinServerRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void SetSkinServerRpc()
    {
        manager.clickButton.GetComponent<Image>().sprite = image.sprite;
    }
}
