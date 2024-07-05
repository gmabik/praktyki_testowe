using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Netcode;
using Netcode.Transports.Facepunch;
using Unity.Networking;
using UnityEngine.UI;
using Steamworks;

public class CursorScript : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerName;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            SetNameClientRpc(SteamClient.Name);
        }
    }
    [ClientRpc]
    public void SetNameClientRpc(string name)
    {
        playerName.text = name;
    }


    private void Update()
    {
        Vector2 newPos = Vector2.one;
        if (IsOwner)
        {
            newPos = (Vector2)Input.mousePosition / transform.parent.GetComponent<Canvas>().scaleFactor;
        }
        UpdatePosClientRpc(newPos);
    }
    [ClientRpc]
    public void UpdatePosClientRpc(Vector2 newPos)
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = newPos;
    }
}
