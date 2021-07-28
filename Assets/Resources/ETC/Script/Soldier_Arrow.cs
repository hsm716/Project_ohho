using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Soldier_Arrow : MonoBehaviourPunCallbacks
{
    Rigidbody rgbd;
    public PhotonView PV;
    Soldier mySoldier;
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
        FindMySoldier();
        atk = mySoldier.atk;
        rgbd.AddForce(transform.forward * 35f, ForceMode.Impulse);
        Invoke("DestroyRPC", 3f);

    }
    void FindMySoldier()
    {
        GameObject[] soldiers = GameObject.FindGameObjectsWithTag("Soldier");
        foreach (GameObject p in soldiers)
        {
            if (p.GetComponent<Soldier>().soldierType==Soldier.Type.arrow&&p.GetComponent<Soldier>().PV.Owner == PV.Owner)
            {
                mySoldier = p.GetComponent<Soldier>();
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {


        if (!PV.IsMine && col.CompareTag("Player") && col.GetComponent<PhotonView>().IsMine)
        {
            col.GetComponent<Player_Control>().Hit(atk);
            Debug.Log(col.gameObject.name + "를 맞춤 " + "데미지 : " + atk);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        if (col.CompareTag("Soldier") && col.GetComponent<PhotonView>().Owner != PV.Owner)
        {
            col.GetComponent<Soldier>().Hit(atk);
            Debug.Log(col.gameObject.name + "를 맞춤 " + "데미지 : " + atk);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        if (col.CompareTag("Ground"))
        {
            rgbd.isKinematic = true;
        }
    }
    [PunRPC]
    void DestroyRPC() => Destroy(this.gameObject);

}
