using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SkinScript : MonoBehaviour
{
    public bool isUnlocked { get; private set; }
    private Image image;
    public SkinsManager manager;
    public int skinDataNum;
    public SkinSO skinData;

    public void OnSpawn()
    {
        skinData = manager.skinDatas[skinDataNum];
        image = GetComponent<Image>();
        image.sprite = skinData.sprite;
        isUnlocked = false;
        image.color = Color.black;
    }

    public void Unlock()
    {
        isUnlocked = true;
        image.color = Color.white;
    }

    public void OnClick()
    {
        if (!isUnlocked) return;
        manager.SpawnNewSkinRpc(skinDataNum);
    }
}
