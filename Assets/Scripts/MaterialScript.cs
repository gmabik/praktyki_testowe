using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class MaterialScript : MonoBehaviour
{
    public bool isUnlocked { get; private set; }
    private Image image;
    public SkinsManager manager;
    public int matDataNum;
    public MaterialSO matData;

    public void OnSpawn()
    {
        matData = manager.matDatas[matDataNum];
        image = GetComponent<Image>();
        image.sprite = matData.sprite;
        if(matDataNum != 0)
        {
            isUnlocked = false;
            image.color = Color.black;
        }
        else isUnlocked = true;
    }

    public void Unlock()
    {
        isUnlocked = true;
        image.color = Color.white;
    }

    public void OnClick()
    {
        if (!isUnlocked) return;
        manager.currentMat = matData.mat;
        manager.SetMaterialRpc();
    }
}
