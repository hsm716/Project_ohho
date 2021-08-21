using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using UnityEditor;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;

public class PlayerManager : MonoBehaviour,IPunObservable
{
    public PhotonView PV;
    public GameObject MC;
    Player[] players;

    public string getValue;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
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
            if (PV.IsMine)
            {

                CreateController();

            }
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
        PhotonView pv = PhotonNetwork.Instantiate("Player", new Vector3(-2, 23.5f, -4.1f), Quaternion.identity,0).GetComponent<PhotonView>();
        pv.transform.GetChild(0).GetComponent<Player_Control>().atk = 5000;
        pv.transform.GetChild(0).GetComponent<Player_Control>().getValue = getValue;
        MC.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(getValue);
        }
        else
        {
            getValue = (string)stream.ReceiveNext();
        }
    }
}
