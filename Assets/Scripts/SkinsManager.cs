using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Steamworks;
using DG.Tweening;
using System.Linq;
using Steamworks.Data;
using System;
using System.Threading.Tasks;

public class SkinsManager : NetworkBehaviour
{
    [Header("Skins")]
    [SerializeField] private GameObject skinItemPrefab;
    [SerializeField] private Transform skinGridParent;
    public GameObject currentSkin;
    public List<SkinSO> skinDatas;
    public List<GameObject> skinButtons;
    public Dictionary<int, InventoryDef> defsWithPrices;
    private Task dictTask;

    [Header("Mats")]
    [SerializeField] private GameObject matItemPrefab;
    [SerializeField] private Transform matGridParent;
    public Material currentMat;
    public List<MaterialSO> matDatas;
    public List<GameObject> matButtons;
    [SerializeField] private GameObject redDot;

    [Header("Misc")]
    [SerializeField] private Transform canvas;
    [SerializeField] private Transform spawnPos;

    private void Awake()
    {
        //await RemoveAllItems();
        dictTask = Task.Run(() => MakeDictionary());
    }

    
    public async void Start()
    {
        //SteamInventory.OnInventoryUpdated += UpdateInventory;

        matGridParent.parent.parent.gameObject.SetActive(false);
        skinGridParent.parent.parent.gameObject.SetActive(false);

        //await SteamInventory.GetAllItemsAsync();

        for (int i = 0; i < matDatas.Count; i++)
        {
            GameObject mat = Instantiate(matItemPrefab);
            mat.transform.SetParent(matGridParent);
            matButtons.Add(mat);
            mat.GetComponent<MaterialScript>().matDataNum = i;
            mat.GetComponent<MaterialScript>().manager = this;
            mat.GetComponent<MaterialScript>().OnSpawn();
        }

        await dictTask;

        for (int i = 0; i < skinDatas.Count; i++)
        {
            GameObject skin = Instantiate(skinItemPrefab);
            skin.transform.SetParent(skinGridParent);
            skinButtons.Add(skin);
            skin.GetComponent<SkinScript>().skinDataNum = i;
            skin.GetComponent<SkinScript>().manager = this;
            skin.GetComponent<SkinScript>().OnSpawn();
        }

        StartCoroutine(GrantStarterSkins());

        currentSkin.GetComponent<MeshRenderer>().material = matDatas[0].mat;
        currentMat = matDatas[0].mat;
    }

    private async Task MakeDictionary()
    {
        defsWithPrices = new Dictionary<int, InventoryDef>();
        InventoryDef[] items = await SteamInventory.GetDefinitionsWithPricesAsync();
        defsWithPrices = ConvertToDict(items);
    }


    private IEnumerator GrantStarterSkins()
    {
        GrantStarter(10, 30);
        yield return new WaitForSeconds(2);
        GrantStarter(20, 32);
    }

    private async void GrantStarter(int itemID, int generatorID)
    {
        List<InventoryItem> starterSkins = CheckIfHasItem(itemID);
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
            InventoryDefId genID = new() { Value = generatorID };
            InventoryResult? result = await SteamInventory.TriggerItemDropAsync(genID);
            InventoryItem[] items = result.Value.GetItems();
            foreach (InventoryItem item in items)
            {
                int id = item.DefId.Value;
                Debug.LogError(id);

                string stringID = id.ToString();
                int num = Convert.ToInt32(stringID[1..]);

                if (stringID[0] == '2') skinButtons[num].GetComponent<Item>().StartAcquire();
                else matButtons[num].GetComponent<Item>().StartAcquire();
            }
        }
    }

    public List<InventoryItem> CheckIfHasItem(int id)
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
        InventoryDefId genID = new() { Value = idNum };
        InventoryResult? result = await SteamInventory.TriggerItemDropAsync(genID);
        RedDotSetActive(true);
        InventoryItem[] items = result.Value.GetItems();
        foreach (InventoryItem item in items)
        {
            int id = item.DefId.Value;
            Debug.LogError(id);
            int num = Convert.ToInt32(id.ToString()[1..]);
            matButtons[num].GetComponent<MaterialScript>().StartAcquire();
        }
    }

    public IEnumerator CheckItemAcquisition(Item script)
    {
        GameObject image = script.reloadImage;
        while (!script.isAcquired)
        {
            image.SetActive(true);
            image.transform.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
            script.UpdateUnlockStatus();
            yield return new WaitForSeconds(1f);
            script.reloadImage.SetActive(false);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SetMaterialRpc()
    {
        currentSkin.GetComponent<MeshRenderer>().material = currentMat;
    }

    [Rpc(SendTo.Everyone)]
    public void SetMaterialRpc(int matDataNum)
    {
        currentMat = matDatas[matDataNum].mat;
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
        isMatPanelOpened = !isMatPanelOpened;
        matGridParent.parent.parent.gameObject.SetActive(!matGridParent.parent.parent.gameObject.activeSelf);
        RedDotSetActive(false);
        if (isSkinPanelOpened) OpenCloseSkinPanel();
    }

    private bool isSkinPanelOpened;
    public void OpenCloseSkinPanel()
    {
        isSkinPanelOpened = !isSkinPanelOpened;
        skinGridParent.parent.parent.gameObject.SetActive(!skinGridParent.parent.parent.gameObject.activeSelf);
        if (isMatPanelOpened) OpenCloseMatPanel();
    }

    public void RedDotSetActive(bool state)
    { 
        redDot.SetActive(state);
    }
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

    private async Task RemoveAllItems() //purely for testing
    {
        InventoryItem[] items = SteamInventory.Items;
        if (items == null) return;
        foreach (InventoryItem item in items)
        {
            await item.ConsumeAsync(1);
        }
    }
}
