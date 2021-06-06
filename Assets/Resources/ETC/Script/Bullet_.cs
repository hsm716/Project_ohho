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

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Ground") || col.CompareTag("wall"))
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);

        if (!PV.IsMine && col.CompareTag("Player") && col.GetComponent<PhotonView>().IsMine)
        {
            col.GetComponent<Player_Control>().Hit();
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

}
