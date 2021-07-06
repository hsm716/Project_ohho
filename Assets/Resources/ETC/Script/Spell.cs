using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviourPunCallbacks
{
    Rigidbody rgbd;
    public PhotonView PV;
    Player_Control myPlayer;
    public float atk;



    /*[PunRPC]
    public void Shoot(Vector3 dir)
    {
        rgbd.isKinematic = false;
        direction = dir;
        rgbd.AddForce(direction * 50f, ForceMode.Impulse);
        Invoke("DestroyArrow", 3f);
    }*/
    void Awake()
    {
        rgbd = GetComponent<Rigidbody>();
        rgbd.isKinematic = false;
        atk = myPlayer.atk;
        rgbd.AddForce(transform.forward * 30f, ForceMode.Impulse);
        Invoke("DestroyRPC", 3f);

    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Ground"))
        {
            rgbd.isKinematic = true;
            PhotonNetwork.Instantiate("Explosion", transform.position, transform.rotation);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    void DestroyRPC() => Destroy(this.gameObject);

}

