using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public abstract class Item : MonoBehaviour
{
    public bool IsUnlocked { get; protected set; }
    public Image imageComponent;
    public SkinsManager manager;
    public InventoryDef itemDef;
    [SerializeField] protected TMP_Text text;
    public GameObject reloadImage;

    public void OnSpawn()
    {
        //image = GetComponent<Image>();
        UpdateUnlockStatus();
        ChangeScale();
    }

    public abstract void UpdateUnlockStatus();

    public bool isAcquired;
    public void StartAcquire()
    {
        isAcquired = false;
        manager.StartCoroutine(manager.CheckItemAcquisition(this));
    }

    private void OnEnable()
    {
        //UpdateUnlockStatus();
        if (isAcquired) reloadImage.SetActive(false);
    }

    private void ChangeScale()
    {
        transform.localScale = new(0.7f, 0.7f, 0.7f);
    }
}
