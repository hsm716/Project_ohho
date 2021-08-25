using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using UnityEditor;
using System.Linq;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    PhotonView PV;
    public GameObject MC;
    Player[] players;

    public int[] Preset_test;
    public string preset_string;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        preset_string = "1000";

    }

    void Start()
    {
        RoomManager.Instance.playerCount++;
        MC = GameObject.Find("MainCanvas");
        players = PhotonNetwork.PlayerList;

    }
    bool spawnCheck = true;
    void Update()
    {
        if(RoomManager.Instance.playerCount >= players.Count() && spawnCheck)
        {
            spawnCheck = false;
            GameManager.Instance.isStart = true;
            GameManager.Instance.doStartCamera = true;


        }
        if (PV.IsMine && GameManager.Instance.doPlayerSpawn)
        {
            GameManager.Instance.doPlayerSpawn = false;
            CreateController();

        }
    }

    void CreateController()
    {
        //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(-2,23.5f,-4.1f), Quaternion.identity);
        StartCoroutine(LateSpawn());
    }

    IEnumerator LateSpawn()
    {
        yield return new WaitForSeconds(0);
        GameObject PC = PhotonNetwork.Instantiate("Player", new Vector3(-2, 23.5f, -4.1f), Quaternion.identity);
        PC.transform.GetChild(0).GetComponent<Player_Control>().preset_data = preset_string;
        MC.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(preset_string);
        }
        else
        {
            preset_string = (string)stream.ReceiveNext();
        }
    }
}
