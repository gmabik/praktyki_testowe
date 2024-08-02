using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Steamworks;
using TMPro;

public class MaterialScript : MonoBehaviour
{
    public bool IsUnlocked { get; private set; }
    private Image image;
    public SkinsManager manager;
    public int matDataNum;
    public MaterialSO matData;
    public InventoryDef itemDef;
    private int currentAmount;
    [SerializeField] private TMP_Text amountText;

    public void OnSpawn()
    {
        matData = manager.matDatas[matDataNum];
        itemDef = new InventoryDef(matData.id);
        image = GetComponent<Image>();
        image.sprite = matData.sprite;
        UpdateAmount();
        UpdateUIAmount();
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
            UpdateAmount();
        }
        else
        {
            manager.currentMat = matData.mat;
            manager.SetMaterialRpc();
        }
    }

    public async void UpdateAmount()
    {
        await SteamInventory.GetAllItemsAsync();
        var list = manager.CheckIfHasItem(matData.id);
        if (currentAmount != list.Count)
        {
            currentAmount = list.Count;
            UpdateUIAmount();
        }
    }

    private void UpdateUIAmount()
    {
        if (currentAmount == 0)
        {
            IsUnlocked = false;
            image.color = Color.black;
            amountText.gameObject.SetActive(false);
        }
        else
        {
            Unlock();
            amountText.gameObject.SetActive(true);
            amountText.text = "x" + currentAmount;
        }
    }
}
