using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Steamworks;
using DG.Tweening;
using System.Linq;
using Steamworks.Data;

public class SkinsManager : NetworkBehaviour
{
    [SerializeField] private GameObject skinItemPrefab;
    [SerializeField] private GameObject matItemPrefab;
    [SerializeField] private Transform matGridParent;
    [SerializeField] private Transform skinGridParent;
    [SerializeField] private Transform canvas;
    public GameObject currentSkin;
    public Material currentMat;
    [SerializeField] private Transform spawnPos;
    public List<SkinSO> skinDatas;
    public List<MaterialSO> matDatas;
    public List<GameObject> skinButtons;
    public List<GameObject> matButtons;
    [SerializeField] private GameObject redDot;


    public Dictionary<int, InventoryDef> defsWithPrices;

    private async void Start()
    {
        //RemoveAllItems();
        matGridParent.parent.parent.gameObject.SetActive(false);
        skinGridParent.parent.parent.gameObject.SetActive(false);
        await SteamInventory.GetAllItemsAsync();
        defsWithPrices = ConvertToDict(await SteamInventory.GetDefinitionsWithPricesAsync());
        GrantStarterSkin();
        for (int i = 0; i < matDatas.Count; i++)
        {
            GameObject mat = Instantiate(matItemPrefab);
            mat.transform.SetParent(matGridParent);
            matButtons.Add(mat);
            mat.GetComponent<MaterialScript>().matDataNum = i;
            mat.GetComponent<MaterialScript>().manager = this;
            mat.GetComponent<MaterialScript>().OnSpawn();
        }
        for (int i = 0; i < skinDatas.Count; i++)
        {
            GameObject skin = Instantiate(skinItemPrefab);
            skin.transform.SetParent(skinGridParent);
            skinButtons.Add(skin);
            skin.GetComponent<SkinScript>().skinDataNum = i;
            skin.GetComponent<SkinScript>().manager = this;
            skin.GetComponent<SkinScript>().OnSpawn();
        }
        currentSkin.GetComponent<MeshRenderer>().material = matDatas[0].mat;
        currentMat = matDatas[0].mat;
    }

    private async void GrantStarterSkin()
    {
        List<InventoryItem> starterSkins = CheckIfHasMat(10);
        foreach (InventoryItem item in starterSkins)
        {
            if (starterSkins.Count > 1)
            {
                await item.ConsumeAsync(1);
                starterSkins.Remove(item);
            }
        }
        if (starterSkins.Count == 0)
        {
            GrantItem(30);
        }
    }

    public List<InventoryItem> CheckIfHasMat(int id)
    {
        InventoryItem[] items = SteamInventory.Items;
        if (items == null) return new();
        List<InventoryItem> skins = new();
        foreach (InventoryItem item in items)
        {
            //await item.ConsumeAsync(1);
            //print(item.DefId.Value + "     " + item.Quantity + "     " + item.IsConsumed);
            if (item.DefId.Value == id)
            {
                skins.Add(item);
            }
        }
        return skins;
    }

    public async void GrantItem(int idNum)
    {
        InventoryDefId id = new() { Value = idNum };
        InventoryResult? result = await SteamInventory.TriggerItemDropAsync(id);
        print(result.Value);
    }

    [Rpc(SendTo.Everyone)]
    public void SetMaterialRpc()
    {
        currentSkin.GetComponent<MeshRenderer>().material = currentMat;
    }

    [Rpc(SendTo.Owner)]
    public void SpawnNewSkinRpc(int i)
    {
        GameObject skin = Instantiate(skinDatas[i].model.Prefab);
        skin.AddComponent<NetworkObject>().Spawn();
        if (currentSkin != null) currentSkin.GetComponent<NetworkObject>().Despawn(true);
        SetNewTransformRpc(skin.GetComponent<NetworkObject>());
    }

    [Rpc(SendTo.Everyone)]
    public void SetNewTransformRpc(NetworkObjectReference _skin)
    {
        if (_skin.TryGet(out NetworkObject skin))
        {
            skin.transform.position = spawnPos.position;
            skin.transform.rotation = spawnPos.rotation;
            skin.transform.localScale = spawnPos.localScale;
            currentSkin = skin.gameObject;
            SetMaterialRpc();
        }
    }

    public void ClickAnim()
    {
        this.StopAllCoroutines();
        currentSkin.transform.DOComplete();
        currentSkin.transform.DOShakeRotation(0.5f, 10f, 3);
        //StartCoroutine(PlayClickAnim());
    }

    /*private IEnumerator PlayClickAnim()
    {
        Transform skin = currentSkin.transform;
        skin.DORotate(spawnPos.rotation.eulerAngles + new Vector3(5, 0, 0), 0.2f);
        yield return new WaitForSeconds(0.2f);
        skin.DORotate(spawnPos.rotation.eulerAngles - new Vector3(5, 0, 0), 0.4f);
        yield return new WaitForSeconds(0.4f);
        skin.DORotate(spawnPos.rotation.eulerAngles, 0.2f);
    }*/

    #region
    private bool isMatPanelOpened;
    public void OpenCloseMatPanel()
    {
        if (!isMatPanelOpened)
        {
            isMatPanelOpened = true;
            matGridParent.parent.parent.gameObject.SetActive(true);
            if(isSkinPanelOpened) OpenCloseSkinPanel();
            RedDotSetActive(false);
        }
        else
        {
            isMatPanelOpened = false;
            matGridParent.parent.parent.gameObject.SetActive(false);
        }
    }

    private bool isSkinPanelOpened;
    public void OpenCloseSkinPanel()
    {
        if (!isSkinPanelOpened)
        {
            isSkinPanelOpened = true;
            skinGridParent.parent.parent.gameObject.SetActive(true);
            if(isMatPanelOpened) OpenCloseMatPanel();
        }
        else
        {
            isSkinPanelOpened = false;
            skinGridParent.parent.parent.gameObject.SetActive(false);
        }
    }

    public void RedDotSetActive(bool state)
        => redDot.SetActive(state);
    #endregion

    public Dictionary<int, InventoryDef> ConvertToDict(InventoryDef[] defs)
    {
        Dictionary<int, InventoryDef> dict = new();
        foreach (InventoryDef def in defs)
        {
            dict.Add(def.Id, def);
        }
        return dict;
    }

    private async void RemoveAllItems() //purely for testing
    {
        InventoryItem[] items = SteamInventory.Items;
        if(items == null) return;
        foreach (InventoryItem item in items)
        {
            await item.ConsumeAsync(1);
        }
    }
}
