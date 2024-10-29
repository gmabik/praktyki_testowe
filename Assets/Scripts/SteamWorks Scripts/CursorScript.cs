using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;
using Steamworks;
using Unity.Collections;
using Random = UnityEngine.Random;

public class CursorScript : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    public NetworkVariable<FixedString64Bytes> ownerName = new NetworkVariable<FixedString64Bytes>();
    public NetworkVariable<Color32> playerColor = new NetworkVariable<Color32>();
    [SerializeField] private Sprite cursorSprite;
    public CursorManager cursorManager;

    public override void OnNetworkSpawn()
    {
        //base.OnNetworkSpawn();
        playerNameText.raycastTarget = false;
        if (IsOwner)
        {
            ownerName.Value = SteamClient.Name;
            playerColor.Value = CreateColor();

            SetDataRpc();
            gameObject.GetComponent<Image>().enabled = false;
            playerNameText.enabled = false;
            cursorManager.OnCursorSpawnComplete(this);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SetDataRpc()
    {
        playerNameText.text = ownerName.Value.ToString();
        GetComponent<Image>().sprite = cursorSprite;
        GetComponent<Image>().color = playerColor.Value;
    }

    private Color32 CreateColor()
    {
        byte r = (byte) Random.Range(0, 255);
        byte g = (byte) Random.Range(0, 255);
        byte b = (byte) Random.Range(0, 255);

        return new Color32(r, g, b, 255);
    }

    private void Update()
    {
        if (IsOwner)
        {
            Vector2 newPos = (Vector2)Input.mousePosition / transform.parent.GetComponent<Canvas>().scaleFactor;
            UpdatePosRpc(newPos);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void UpdatePosRpc(Vector2 newPos)
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = newPos;
    }
}
