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
        print(itemDef == null);
        print(itemDef.LocalPrice + "       " + itemDef.LocalBasePrice);
        image = GetComponent<Image>();
        image.sprite = skinData.sprite;
        priceText.text = itemDef.LocalPriceFormatted;
    }

    public void Unlock()
    {
        IsUnlocked = true;
        image.color = Color.white;
    }

    public void OnClick()
    {
        if (!IsUnlocked)
        {

        }
        else manager.SpawnNewSkinRpc(skinDataNum);
    }
}
