﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Player_remove : MonoBehaviourPunCallbacks
{
    public PhotonView PV;


    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.childCount == 1)
        {
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);
}
