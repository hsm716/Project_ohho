using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Arrow : MonoBehaviourPunCallbacks, IPunObservable
{
    Rigidbody rgbd;
    public PhotonView PV;
    public Player_Control myPlayer;
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
        atk = 50f;
        rgbd.isKinematic = false;
        rgbd.AddForce(transform.forward * 20f, ForceMode.Impulse);
        Invoke("DestroyRPC", 3f);

    }

    private void OnTriggerEnter(Collider col)
    {


        if (!PV.IsMine && col.CompareTag("Player") && col.GetComponent<PhotonView>().IsMine)
        {
            col.GetComponent<Player_Control>().Hit(atk);
            Debug.Log(col.gameObject.name+"를 맞춤 "+"데미지 : " + atk);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        if (col.CompareTag("Ground"))
        {
            rgbd.isKinematic = true;
        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(atk);

        }
        else
        {
            atk = (float)stream.ReceiveNext();

        }
    }

}
