using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviourPunCallbacks
{
    Rigidbody rgbd;
    public PhotonView PV;
    public Player_Control myPlayer;
    public float atk;

    public GameObject boom;
    Vector3 dir_;
    Vector3 dir;

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
        dir_ = myPlayer.mouseDir_y;

        //rgbd.AddForce( dir_*3.5f/** 10f*/, ForceMode.Impulse);
        boom.GetComponent<SpellExplosion>().atk = atk;
        StartCoroutine("Boom");
        Invoke("DestroyRPC", 1.0f);

    }
    private void Update()
    {
        transform.position = Vector3.Slerp(transform.position, myPlayer.transform.position +dir_, 0.05f);
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
        if (col.CompareTag("Ground")||(col.CompareTag("Player") && col.GetComponent<Player_Control>().PV.Owner != PV.Owner))
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
    }
    [PunRPC]
    void DestroyRPC()
    {
        Destroy(this.gameObject);
    }

}

