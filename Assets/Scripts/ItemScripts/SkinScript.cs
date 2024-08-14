using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks;
using TMPro;

public class SkinScript : Item
{
    public int skinDataNum;
    public SkinSO skinData;

    public new void OnSpawn()
    {
        skinData = manager.skinDatas[skinDataNum];
        manager.defsWithPrices.TryGetValue(skinData.id, out itemDef);
        base.OnSpawn();
        image.sprite = skinData.sprite;
    }

    public override void UpdateUnlockStatus()
    {
        SteamInventory.GetAllItems();
        if (manager.CheckIfHasItem(skinData.id).Count > 0)
        {
            //print("count > 0");
            text.text = "Owned";
            IsUnlocked = true;
            isAcquired = true;
        }
        else
        {
            text.text = itemDef.LocalPriceFormatted;
            IsUnlocked = false;
            //print("count = 0");
        }
    }

    public void OnClick()
    {
        if (!IsUnlocked)
        {
            manager.gameObject.GetComponent<PurchaseManager>().StartPurchase(itemDef);
            UpdateUnlockStatus();
        }
        else manager.SpawnNewSkinRpc(skinDataNum);
    }
}
