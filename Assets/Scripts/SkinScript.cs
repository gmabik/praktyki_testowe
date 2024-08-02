using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks;
using TMPro;

public class SkinScript : MonoBehaviour
{
    public bool IsUnlocked { get; private set; }
    private Image image;
    public SkinsManager manager;
    public int skinDataNum;
    public SkinSO skinData;
    public InventoryDef itemDef;
    [SerializeField] private TMP_Text priceText;

    public void OnSpawn()
    {
        skinData = manager.skinDatas[skinDataNum];
        manager.defsWithPrices.TryGetValue(skinData.id, out itemDef);
        //print(itemDef.LocalPrice + "       " + itemDef.LocalBasePrice);
        image = GetComponent<Image>();
        image.sprite = skinData.sprite;
        StartUnlock();
    }

    public async void StartUnlock()
    {
        await SteamInventory.GetAllItemsAsync();
        if (manager.CheckIfHasItem(skinData.id).Count > 0)
        {
            print("count > 0");
            priceText.text = "Owned";
            IsUnlocked = true;
        }
        else
        {
            priceText.text = itemDef.LocalPriceFormatted;
            IsUnlocked = false;
            print("count = 0");
        }
    }

    public void OnClick()
    {
        if (!IsUnlocked)
        {
            manager.gameObject.GetComponent<PurchaseManager>().StartPurchase(itemDef);
            StartUnlock();
        }
        else manager.SpawnNewSkinRpc(skinDataNum);
    }
}
