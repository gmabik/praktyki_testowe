using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SkinsManager : NetworkBehaviour
{
    [SerializeField] private List<SkinSO> skins;
    [SerializeField] private GameObject skinItemPrefab;
    public Transform gridParent;
    [SerializeField] private GameObject currentSkin;
    [SerializeField] private Transform spawnPos;

    private void Start()
    {
        if(!IsHost) return;
        foreach (var skinData in skins)
        {
            GameObject skin = Instantiate(skinItemPrefab);
            skin.GetComponent<SkinScript>().skinData = skinData;
            skin.GetComponent<NetworkObject>().Spawn();
            if (!skin.GetComponent<NetworkObject>().TrySetParent(gridParent, false)) print("couldnt parent");
            //ParentAndAsignDataRpc(skin.GetComponent<NetworkObject>()/*, skinData*/);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ParentAndAsignDataRpc(NetworkObjectReference skinReference/*, SkinSO _skinData*/)
    {
        if (skinReference.TryGet(out NetworkObject skin))
        {
            
            //skin.GetComponent<SkinScript>().skinData = _skinData;
        }
    }

    [Rpc(SendTo.Owner)]
    public void SpawnNewSkinRpc(NetworkObjectReference _skin)
    {
        if (!_skin.TryGet(out NetworkObject skin)) return;
        GameObject newSkin = Instantiate(skin.gameObject);
        newSkin.AddComponent<NetworkObject>().Spawn();
        currentSkin.GetComponent<NetworkObject>().Despawn(true);
    }

    [Rpc(SendTo.Everyone)]
    public void SetPosForNewSkinRpc(NetworkObjectReference _newSkin)
    {
        if (_newSkin.TryGet(out NetworkObject newSkin))
        {
            newSkin.transform.position = spawnPos.position;
            newSkin.transform.eulerAngles = spawnPos.eulerAngles;
            newSkin.transform.localScale = spawnPos.localScale;
        }
    }

    #region
    private bool isSkinPanelOpened;
    public void OnClick()
    {
        if (!isSkinPanelOpened)
        {
            isSkinPanelOpened = true;
            gridParent.gameObject.SetActive(true);
        }
        else
        {
            isSkinPanelOpened = false;
            gridParent.gameObject.SetActive(false);
        }
    }
    #endregion
}
