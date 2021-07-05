using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet_ : MonoBehaviourPunCallbacks
{
    Rigidbody rgbd;
    public PhotonView PV;
    float ShotSPD = 50f;
    void Start()
    {
        rgbd = GetComponent<Rigidbody>();
        rgbd.AddForce(transform.forward * ShotSPD, ForceMode.Impulse);
        Destroy(gameObject, 3.5f);
    
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

}
