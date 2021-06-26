using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Arrow : MonoBehaviourPunCallbacks
{
    public Rigidbody rgbd;
    public PhotonView PV;
    private Vector3 direction;

    [PunRPC]
    public void Shoot(Vector3 dir)
    {
        rgbd.isKinematic = false;
        direction = dir;
        rgbd.AddForce(direction*50f, ForceMode.Impulse);
        Invoke("DestroyArrow", 5f);
    }
    [PunRPC]
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
      

        if (!PV.IsMine && col.CompareTag("Player"))
        {
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

}
