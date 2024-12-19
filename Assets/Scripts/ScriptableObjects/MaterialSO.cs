using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialData")]
public class MaterialSO : ScriptableObject
{
    public Material mat;
    public Sprite sprite;
    public int id; // steam id, which you get when adding an item to steam
    public GameObject effect;
}
