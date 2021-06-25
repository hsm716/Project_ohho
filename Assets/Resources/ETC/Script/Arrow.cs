using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Arrow : MonoBehaviourPunCallbacks
{
    Rigidbody rgbd;
    public PhotonView PV;
    private Vector3 direction;

    public void Shoot(Vector3 dir)
    {
        direction = dir;
        rgbd.AddForce(direction*50f, ForceMode.Impulse);
        Invoke("DestroyArrow", 5f);
    }
    public void DestroyArrow()
    {
        Player_Control.ReturnArrow(this);
    }
    void Awake()
    {
        rgbd = GetComponent<Rigidbody>();


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
