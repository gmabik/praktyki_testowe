using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks;
using TMPro;

public class MaterialScript : Item
{
    public int matDataNum;
    public MaterialSO matData;
    private int currentAmount;

    public new void OnSpawn()
    {
        matData = manager.matDatas[matDataNum];
        itemDef = new InventoryDef(matData.id);
        base.OnSpawn();
        image.sprite = matData.sprite;
        UpdateUIAmount();
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
        }
    }

    public override void UpdateUnlockStatus()
    {
        SteamInventory.GetAllItems();
        var list = manager.CheckIfHasItem(matData.id);
        if (currentAmount != list.Count)
        {
            currentAmount = list.Count;
            UpdateUIAmount();
            isAcquired = true;
        }
    }

    private void UpdateUIAmount()
    {
        if (currentAmount == 0)
        {
            IsUnlocked = false;
            image.color = Color.black;
            text.gameObject.SetActive(false);
        }
        else
        {
            IsUnlocked = true;
            image.color = Color.white;
            text.gameObject.SetActive(true);
            text.text = "x" + currentAmount;
        }
    }
}
