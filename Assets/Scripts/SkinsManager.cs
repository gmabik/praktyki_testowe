using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public class SkinsManager : NetworkBehaviour
{
    [SerializeField] private GameObject skinItemPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Transform canvas;
    public GameObject currentSkin;
    [SerializeField] private Transform spawnPos;
    [SerializeField] private List<SkinSO> skinDatas;
    public List<GameObject> skinButtons;

    private void Start()
    {
        gridParent.parent.parent.gameObject.SetActive(false);
        if (!IsHost) return;
        /*gridParent.GetComponent<NetworkObject>().Spawn();
        gridParent.SetParent(canvas, false);*/
        for (int i = 0; i < skinDatas.Count; i++)
        {
            GameObject skin = Instantiate(skinItemPrefab);
            skin.GetComponent<NetworkObject>().Spawn();
            skin.transform.SetParent(gridParent);
            skinButtons.Add(skin);
            AsignDataRpc(skin.GetComponent<NetworkObject>(), i);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void AsignDataRpc(NetworkObjectReference skinReference, int i)
    {
        if (skinReference.TryGet(out NetworkObject skin))
        {
            skin.GetComponent<SkinScript>().skinData = skinDatas[i];
            skin.GetComponent<SkinScript>().manager = this;
            skin.GetComponent<SkinScript>().SetDataRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    public void NewSkinSetTransformRpc(NetworkObjectReference _skin)
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
        }
        currentSkin = skin.gameObject;
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

    public void ClickAnim()
    {
        this.StopAllCoroutines();
        currentSkin.transform.DOComplete();
        //currentSkin.transform.DOShakeRotation(0.5f, 10f, 3);
        StartCoroutine(PlayClickAnim());
    }

    private IEnumerator PlayClickAnim()
    {
        Transform skin = currentSkin.transform;
        skin.DORotate(spawnPos.rotation.eulerAngles + new Vector3(5, 0, 0), 0.2f);
        yield return new WaitForSeconds(0.2f);
        skin.DORotate(spawnPos.rotation.eulerAngles - new Vector3(5, 0, 0), 0.4f);
        yield return new WaitForSeconds(0.4f);
        skin.DORotate(spawnPos.rotation.eulerAngles, 0.2f);
    }

    #region
    private bool isSkinPanelOpened;
    public void OnClick()
    {
        if (!isSkinPanelOpened)
        {
            isSkinPanelOpened = true;
            gridParent.parent.parent.gameObject.SetActive(true);
        }
        else
        {
            isSkinPanelOpened = false;
            gridParent.parent.parent.gameObject.SetActive(false);
        }
    }
    #endregion
}
