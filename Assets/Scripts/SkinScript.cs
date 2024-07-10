using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SkinScript : NetworkBehaviour
{
    public bool isUnlocked { get; private set; }
    private Image image;
    public SkinsManager manager;
    public SkinSO skinData;

    public override void OnNetworkSpawn()
    {
        image = GetComponent<Image>();
        image.sprite = skinData.sprite;
        isUnlocked = false;
        image.color = Color.black;
    }

    [Rpc(SendTo.Everyone)]
    public void UnlockRpc()
    {
        isUnlocked = true;
        image.color = Color.white;
    }

    public void OnClick()
    {
        if (isUnlocked) manager.SpawnNewSkinRpc(skinData.model.Prefab);
    }
}
