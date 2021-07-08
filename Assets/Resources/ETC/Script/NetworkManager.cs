using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NickNameInput;
    public GameObject DisconnectPanel;
    public GameObject RespawnPanel;
    Color Head_color;

    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 144;
        PhotonNetwork.SerializationRate = 144;
    }
    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        DisconnectPanel.SetActive(false);
       // StartCoroutine(DestroyUI());
        Spawn();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect(); 
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
    }
  /*  IEnumerator DestroyUI()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (GameObject ui in GameObject.FindGameObjectsWithTag("hp")) ui.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.AllBuffered);
    }
    */
    public void Spawn()
    {
        GameObject obj = PhotonNetwork.Instantiate("Player",new Vector3(Random.Range(-7,8),23.5f,-4.1f), Quaternion.identity);
        RespawnPanel.SetActive(false);
    }
}
