using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellExplosion : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public Player_Control myPlayer;
    public float atk;
    void Awake()
    {
        FindMyPlayer();
        Invoke("DestroyRPC", 1.5f);
    }
    void FindMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PV.Owner.NickName)
            {
                myPlayer = p.GetComponent<Player_Control>();
                break;
            }
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
           if(!PV.IsMine && col.GetComponent<PhotonView>().Owner != PV.Owner)
           {
                col.GetComponent<Player_Control>().Hit(atk, 2);
           }
            //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
            
            col.GetComponent<Player_Control>().Last_Hiter = myPlayer;

        }
        if ((!PV.IsMine && col.CompareTag("Soldier") && col.GetComponent<PhotonView>().Owner != PV.Owner))
        {

            //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
            col.GetComponent<Soldier>().Hit(atk,2);


        }
        if (col.CompareTag("Monster"))
        {

            //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
            col.GetComponent<Monster>().Last_Hiter = myPlayer;
            col.GetComponent<Monster>().Hit(atk,2);
            
            //PV.RPC("Set_LastHiter", RpcTarget.All, col);


        }
    }

    [PunRPC]
    void DestroyRPC() => Destroy(this.gameObject);
}
