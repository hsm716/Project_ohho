using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public Player_Control myPlayer;
    public float atk;

    //public GameObject boom;
    //Vector3 dir_;
    //Vector3 dir;
    //public float distance;


    //Vector3 curPos;
    //Quaternion curRot;

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
        FindMyPlayer();
        atk = myPlayer.atk;
        //StartCoroutine("Boom");
        Invoke("DestroyRPC", 3.0f);

    }

/*    private void Update()
    {
      *//*  if (PV.IsMine)
        {
            transform.position = Vector3.Slerp(transform.position, dir_, 0.008f);
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.fixedDeltaTime * 20f);
            transform.rotation = Quaternion.Lerp(transform.rotation, curRot, Time.fixedDeltaTime * 20f);
        }
*//*


    }*/
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

    private void OnParticleCollision(GameObject col)
    {
        if (col.CompareTag("Player"))
        {
            if (!PV.IsMine && col.GetComponent<PhotonView>().Owner != PV.Owner)
            {
                col.GetComponent<Player_Control>().Hit(atk, 2, myPlayer.curCritical);
            }
            //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));

            col.GetComponent<Player_Control>().Last_Hiter = myPlayer;

        }
        if ((!PV.IsMine && col.CompareTag("Soldier") && col.GetComponent<PhotonView>().Owner != PV.Owner))
        {

            //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
            col.GetComponent<Soldier>().Hit(atk, 2, myPlayer.curCritical);


        }
        if (col.CompareTag("Monster"))
        {

            //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
            col.GetComponent<Monster>().Last_Hiter = myPlayer;
            col.GetComponent<Monster>().Hit(atk, 2, myPlayer.curCritical);

            //PV.RPC("Set_LastHiter", RpcTarget.All, col);


        }
    }


    /*    private void OnTriggerEnter(Collider col)
        {
            if((col.CompareTag("Player") && col.GetComponent<PhotonView>().Owner.NickName != PV.Owner.NickName))
            {
                rgbd.isKinematic = true;
                boom.SetActive(true);
                boom.transform.parent = null;
                //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
                //col.GetComponent<Player_Control>().Hit(atk);

                Invoke("DestroyRPC", 0f);
            }
            if (col.CompareTag("Ground"))
            {
                rgbd.isKinematic = true;
                boom.SetActive(true);
                boom.transform.parent = null;
                //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
                //col.GetComponent<Player_Control>().Hit(atk);

                Invoke("DestroyRPC",0f);
            }
            if ((col.CompareTag("Soldier") && col.GetComponent<Soldier>().PV.Owner != PV.Owner))
            {
                rgbd.isKinematic = true;
                boom.SetActive(true);
                boom.transform.parent = null;
                //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
                //col.GetComponent<Player_Control>().Hit(atk);

                Invoke("DestroyRPC", 0f);
            }
            if (col.CompareTag("Monster"))
            {
                rgbd.isKinematic = true;
                boom.SetActive(true);
                boom.transform.parent = null;
                //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
                //col.GetComponent<Player_Control>().Hit(atk);

                Invoke("DestroyRPC", 0f);
            }
        }

        IEnumerator Boom()
        {
            yield return new WaitForSeconds(0.95f);
            rgbd.isKinematic = true;
            boom.SetActive(true);
            boom.transform.parent = null;
            //PhotonNetwork.Instantiate("Explosion", transform.position, transform.rotation);
        }*/
    [PunRPC]
    void DestroyRPC()
    {
        Destroy(this.gameObject);
    }

/*    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();

        }
    }*/
}

