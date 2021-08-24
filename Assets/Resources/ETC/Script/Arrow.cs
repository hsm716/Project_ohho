using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Arrow : MonoBehaviourPunCallbacks
{
    Rigidbody rgbd;
    public PhotonView PV;
    public Player_Control myPlayer;
    public float atk;
    public float shootPower;
    public ParticleSystem ps;
    Vector3 dir_;


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
        ps.Stop();
        FindMyPlayer();
        atk = myPlayer.atk*(myPlayer.pullPower/20f);
        shootPower =   myPlayer.pullPower/2f +20f;
        var particleMainSettings = ps.main;
        particleMainSettings.startSpeed = shootPower;

        ps.Play();
        
        Invoke("DestroyRPC", 3f);

    }
    void FindMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PV.Owner.NickName)
            {
                myPlayer = p.GetComponent<Player_Control>();
                break;
            }
        }
    }
    private void OnParticleCollision(GameObject col)
    {
        if (col.CompareTag("Player"))
        {
            if (!PV.IsMine && col.GetComponent<PhotonView>().IsMine)
            {
                col.GetComponent<Player_Control>().Hit(atk, 1,myPlayer.curCritical);
            }
            col.GetComponent<Player_Control>().Last_Hiter = myPlayer;

            Debug.Log(col.gameObject.name + "¸¦ ¸ÂÃã " + "µ¥¹ÌÁö : " + atk);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        if (col.CompareTag("Soldier") && col.GetComponent<PhotonView>().Owner != PV.Owner)
        {
            col.GetComponent<Soldier>().Hit(atk, 1, myPlayer.curCritical);
            Debug.Log(col.gameObject.name + "¸¦ ¸ÂÃã " + "µ¥¹ÌÁö : " + atk);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        if (col.CompareTag("Monster"))
        {
            col.GetComponent<Monster>().Last_Hiter = myPlayer;
            col.GetComponent<Monster>().Hit(atk, 1, myPlayer.curCritical);

            Debug.Log(col.gameObject.name + "¸¦ ¸ÂÃã " + "µ¥¹ÌÁö : " + atk);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }
/*    private void OnTriggerEnter(Collider col)
    {


        if (col.CompareTag("Player"))
        {
            if(!PV.IsMine && col.GetComponent<PhotonView>().IsMine){
                col.GetComponent<Player_Control>().Hit(atk, 1);
            }
            col.GetComponent<Player_Control>().Last_Hiter = myPlayer;

            Debug.Log(col.gameObject.name+"¸¦ ¸ÂÃã "+"µ¥¹ÌÁö : " + atk);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        if (col.CompareTag("Soldier")&& col.GetComponent<PhotonView>().Owner != PV.Owner)
        {
            col.GetComponent<Soldier>().Hit(atk,1);
            Debug.Log(col.gameObject.name + "¸¦ ¸ÂÃã " + "µ¥¹ÌÁö : " + atk);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        if (col.CompareTag("Monster"))
        {
            col.GetComponent<Monster>().Last_Hiter = myPlayer;
            col.GetComponent<Monster>().Hit(atk,1);

            Debug.Log(col.gameObject.name + "¸¦ ¸ÂÃã " + "µ¥¹ÌÁö : " + atk);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
        if (col.CompareTag("Ground"))
        {
            rgbd.isKinematic = true;
        }
    }*/



    [PunRPC]
    void DestroyRPC() => Destroy(this.gameObject);

}
