using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Text connectButtonText; 
    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 3)
        {
            PhotonNetwork.NickName = usernameInput.text;
            connectButtonText.text = "Connecting...";
            PhotonNetwork.ConnectUsingSettings();
        }
        else StartCoroutine(WrongName());
    }

    private IEnumerator WrongName()
    {
        connectButtonText.text = "Wrong name";
        yield return new WaitForSeconds(1f);
        connectButtonText.text = "Connect";
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
