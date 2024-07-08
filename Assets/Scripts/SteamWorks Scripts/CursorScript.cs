using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
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
            SetNameRpc(SteamClient.Name);
            gameObject.GetComponent<Image>().enabled = false;
            playerName.enabled = false;
        }
    }
    [Rpc(SendTo.Everyone)]
    public void SetNameRpc(string name)
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
        UpdatePosRpc(newPos);
    }

    [Rpc(SendTo.Everyone)]
    public void UpdatePosRpc(Vector2 newPos)
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = newPos;
    }
}
