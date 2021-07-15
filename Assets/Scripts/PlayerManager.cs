using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

 
    void CreateController()
    {
        //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(-2,23.5f,-4.1f), Quaternion.identity);
        PhotonNetwork.Instantiate("Player", new Vector3(-2,23.5f,-4.1f), Quaternion.identity);
    }
}
