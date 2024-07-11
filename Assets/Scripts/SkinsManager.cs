using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SkinsManager : NetworkBehaviour
{
    [SerializeField] private List<SkinSO> skinDatas;
    [SerializeField] private GameObject skinItemPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Transform canvas;
    [SerializeField] private GameObject currentSkin;
    [SerializeField] private Transform spawnPos;
    public List<GameObject> skins;

    private void Start()
    {
        if(!IsHost) return;
        /*gridParent.GetComponent<NetworkObject>().Spawn();
        gridParent.SetParent(canvas, false);*/
        foreach (var skinData in skinDatas)
        {
            GameObject skin = Instantiate(skinItemPrefab);
            skin.GetComponent<SkinScript>().skinData = skinData;
            skin.GetComponent<SkinScript>().manager = this;
            skin.GetComponent<NetworkObject>().Spawn();
            skin.transform.SetParent(gridParent);
            skins.Add(skin);
            //ParentAndAsignDataRpc(skin.GetComponent<NetworkObject>()/*, skinData*/);
        }
        gridParent.gameObject.SetActive(false);
    }

    [Rpc(SendTo.Everyone)]
    public void ParentAndAsignDataRpc(NetworkObjectReference skinReference/*, SkinSO _skinData*/)
    {
        if (skinReference.TryGet(out NetworkObject skin))
        {
            
            //skin.GetComponent<SkinScript>().skinData = _skinData;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SpawnNewSkinRpc(NetworkObjectReference _skin)
    {
        if (!_skin.TryGet(out NetworkObject skin)) return;
        /*GameObject newSkin = Instantiate(skin.gameObject);
        newSkin.AddComponent<NetworkObject>().Spawn();*/
        skin.transform.position = spawnPos.position;
        skin.transform.rotation = spawnPos.rotation;
        skin.transform.localScale = spawnPos.localScale;
        if (IsHost)
        {
            currentSkin.GetComponent<NetworkObject>().Despawn(true);
            currentSkin = skin.gameObject;
        }
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
