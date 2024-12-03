using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks;
using TMPro;
using System;

public class MaterialScript : Item
{
    public int matDataNum;
    public MaterialSO matData;
    private int currentAmount;
    [SerializeField] private GameObject redDot;

    public new void OnSpawn()
    {
        matData = manager.matDatas[matDataNum];
        itemDef = new InventoryDef(matData.id);
        base.OnSpawn();
        imageComponent.sprite = matData.sprite;
        UpdateUnlockStatus();
    }

    public void OnClick()
    {
        if (!IsUnlocked)
        {
            UpdateUnlockStatus();
        }
        else
        {
            manager.currentMat = matData.mat;
            manager.SetMaterialRpc(matDataNum);
            redDot.SetActive(false);
        }
    }

    private bool startCheck = true;
    public override async void UpdateUnlockStatus()
    {
        await SteamInventory.GetAllItemsAsync();
        var list = manager.CheckIfHasItem(matData.id);
        if (currentAmount != list.Count)
        {
            currentAmount = list.Count;
            isAcquired = true;
            reloadImage.SetActive(false);
            if (startCheck) startCheck = false;
            else redDot.SetActive(true);
        }
        UpdateUIAmount();
    }

    private void UpdateUIAmount()
    {
        if (currentAmount == 0)
        {
            IsUnlocked = false;
            imageComponent.color = Color.black;
            text.gameObject.SetActive(false);
        }
        else
        {
            IsUnlocked = true;
            imageComponent.color = Color.white;
            text.gameObject.SetActive(true);
            text.text = "x" + currentAmount;
        }
    }
}
