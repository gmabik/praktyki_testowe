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
    public List<SkinSO> skinDatas;
    public List<GameObject> skinButtons;

    private void Start()
    {
        gridParent.parent.parent.gameObject.SetActive(false);
        for (int i = 0; i < skinDatas.Count; i++)
        {
            GameObject skin = Instantiate(skinItemPrefab);
            skin.transform.SetParent(gridParent);
            skinButtons.Add(skin);
            skin.GetComponent<SkinScript>().skinDataNum = i;
            skin.GetComponent<SkinScript>().manager = this;
            skin.GetComponent<SkinScript>().OnSpawn();
        }
    }

    [Rpc(SendTo.Owner)]
    public void SpawnNewSkinRpc(int i)
    {
        GameObject skin = Instantiate(skinDatas[i].model.Prefab);
        skin.AddComponent<NetworkObject>().Spawn();
        if(currentSkin != null) currentSkin.GetComponent<NetworkObject>().Despawn(true);
        SetNewTransformRpc(skin.GetComponent<NetworkObject>());
    }

    [Rpc(SendTo.Everyone)]
    public void SetNewTransformRpc(NetworkObjectReference _skin)
    {
        if(_skin.TryGet(out NetworkObject skin))
        {
            skin.transform.position = spawnPos.position;
            skin.transform.rotation = spawnPos.rotation;
            skin.transform.localScale = spawnPos.localScale;
            currentSkin = skin.gameObject;
        }
    }

    public void ClickAnim()
    {
        this.StopAllCoroutines();
        currentSkin.transform.DOComplete();
        currentSkin.transform.DOShakeRotation(0.5f, 10f, 3);
        //StartCoroutine(PlayClickAnim());
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
