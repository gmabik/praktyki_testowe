using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public abstract class Item : MonoBehaviour
{
    // used for skins and materials classes
    public bool IsUnlocked { get; protected set; }
    public Image imageComponent;
    public SkinsManager manager;
    public InventoryDef itemDef;
    [SerializeField] protected TMP_Text text;
    public GameObject reloadImage; //reloading image for skin acquiring

    public void OnSpawn()
    {
        //image = GetComponent<Image>();
        UpdateUnlockStatus();
        ChangeScale();
    }

    public abstract void UpdateUnlockStatus();

    public bool isAcquired;
    public void StartAcquire() // when skin is added to inventory, this function checks when it is actually added and updates ui
    {
        isAcquired = false;
        manager.StartCoroutine(manager.CheckItemAcquisition(this));
    }

    private void OnEnable()
    {
        //UpdateUnlockStatus();
        if (isAcquired) reloadImage.SetActive(false);
    }

    // this can be removed, but there were problems with item scaling when added to grid
    private void ChangeScale()
    {
        transform.localScale = new(0.7f, 0.7f, 0.7f);
    }
}
