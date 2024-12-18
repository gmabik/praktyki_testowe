using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Steamworks;
using DG.Tweening;
using System.Linq;
using Steamworks.Data;
using System.Threading.Tasks;
using System;

public class PurchaseManager : MonoBehaviour
{
    // WASN'T TESTED
    // script for purchase managing for skins, most of it was copied from Facepunch Documentation

    public InventoryDef item;
    private SkinsManager skinManager;

    private void Start()
    {
        skinManager = gameObject.GetComponent<SkinsManager>();
        //
        // Tell this callback to tell us when something has been purchased
        //
        SteamUser.OnMicroTxnAuthorizationResponse += OnPurchaseFinished;
    }

    //
    // Add an item to our cart
    //
    public async void StartPurchase(InventoryDef _item)
    {
        item = _item;
        await CheckoutAsync();
    }

    //
    // Called when they want to check out
    //
    private async Task CheckoutAsync()
    {
        //ShowPurchaseInProgressScreen();
        /*if (item.LocalPrice == 0)
        {

            InventoryResult? result = await SteamInventory.TriggerItemDropAsync(32);

            //if (!result.HasValue) return;
            await SteamInventory.GetAllItemsAsync();
            foreach (InventoryItem _item in result.Value.GetItems())
            {
                int num = Convert.ToInt32(_item.Def.Id.ToString()[1..]);
                print(num);
                skinManager.skinButtons[num].GetComponent<SkinScript>().StartUnlock();
            }
            
        }
        else
        {*/
        // This tries to open the steam overlay to commence the checkout
        var result = await Steamworks.SteamInventory.StartPurchaseAsync(new[] { item });

        Debug.Log($"Result: {result.Value.Result}");
        Debug.Log($"TransID: {result.Value.TransID}");
        Debug.Log($"OrderID: {result.Value.OrderID}");
        //}
    }

    //
    // Called from the callback SteamUser.OnMicroTxnAuthorizationResponse
    //
    private async void OnPurchaseFinished(AppId appid, ulong orderid, bool success)
    {
        //HidePurchaseInProgressScreen();
        await SteamInventory.GetAllItemsAsync();
        if (success)
        {
            int num = Convert.ToInt32(item.Id.ToString()[1..]);
            skinManager.skinButtons[num].GetComponent<SkinScript>().UpdateUnlockStatus();
        }
        else
        {
            // They probably pressed cancel or something
        }
        item = null;
    }
}
