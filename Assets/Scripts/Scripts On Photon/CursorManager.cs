using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using static UnityEngine.UI.GridLayoutGroup;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private GameObject CursorPrefab;
    [SerializeField] private Transform canvas;
    private GameObject myCursor;
    private void Start()
    {
        GameObject cursor = PhotonNetwork.Instantiate("Cursor", CursorPrefab.transform.position, CursorPrefab.transform.rotation);

        cursor.GetComponent<CursorScript>().owner = PhotonNetwork.LocalPlayer;

        myCursor = cursor;
        myCursor.SetActive(false);

        //Debug.Log($"Cursor created for player {PhotonNetwork.LocalPlayer.NickName} with ViewID {cursor.GetComponent<PhotonView>().ViewID}");
        GetComponent<PhotonView>().RPC("SetParentAndName", RpcTarget.AllBuffered, cursor.GetComponent<PhotonView>().ViewID, PhotonNetwork.LocalPlayer.NickName);
    }
    private void Update()
    {
        Vector2 newPos = (Vector2)Input.mousePosition / canvas.GetComponent<Canvas>().scaleFactor;
        myCursor.GetComponent<RectTransform>().anchoredPosition = newPos;
        //Debug.Log($"Local cursor position: {newPos}");
        GetComponent<PhotonView>().RPC("UpdateCursorPos", RpcTarget.Others, myCursor.GetComponent<PhotonView>().ViewID, newPos);
    }

    [PunRPC]
    private void UpdateCursorPos(int cursorID, Vector2 newPos)
    {
        PhotonView view = PhotonView.Find(cursorID);
        if (view != null)
        {
            GameObject cursor = view.gameObject;
            cursor.GetComponent<RectTransform>().anchoredPosition = newPos;
            //Debug.Log($"Updated cursor position for cursorID {cursorID}: {newPos}");
        }
    }

    [PunRPC]
    private void SetParentAndName(int cursorID, string name)
    {
        PhotonView view = PhotonView.Find(cursorID);
        if (view != null)
        {
            GameObject cursor = view.gameObject;
            cursor.transform.SetParent(canvas, false);
            cursor.GetComponentInChildren<TMP_Text>().text = name;
            //Debug.Log($"Parent set for cursorID {cursorID}");
        }
    }
}
