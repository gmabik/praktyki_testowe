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

    [ServerRpc]
    public void Unlock()
    {
        isUnlocked = true;
        image.color = Color.white;
    }

    public void OnClick()
    {
        if(isUnlocked) SetSkinServerRpc();
    }

    [ServerRpc]
    private void SetSkinServerRpc()
    {
        manager.clickButton.GetComponent<Image>().sprite = image.sprite;
    }
}
