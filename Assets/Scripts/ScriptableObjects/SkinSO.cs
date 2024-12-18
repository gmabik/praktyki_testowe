using Steamworks;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinData")]
public class SkinSO : ScriptableObject
{
    public NetworkPrefab model;
    public Sprite sprite;
    public int id; // steam id, which you get when adding an item to steam
}
