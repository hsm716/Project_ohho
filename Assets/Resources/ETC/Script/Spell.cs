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
        FindMyPlayer();
        atk = myPlayer.atk;
        rgbd.isKinematic = false;
        rgbd.AddForce(transform.forward * 10f, ForceMode.Impulse);
        StartCoroutine("Boom");
        Invoke("DestroyRPC", 1.0f);

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
        if (col.CompareTag("Ground")||(!PV.IsMine && col.CompareTag("Player") ))
        {
            rgbd.isKinematic = true;
            PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
            col.GetComponent<Player_Control>().Hit(atk);

            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    IEnumerator Boom()
    {
        yield return new WaitForSeconds(0.97f);
        PhotonNetwork.Instantiate("Explosion", transform.position, transform.rotation);
    }
    [PunRPC]
    void DestroyRPC()
    {
        Destroy(this.gameObject);
    }

}

