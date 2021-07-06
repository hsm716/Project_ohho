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
        rgbd.AddForce(transform.forward * 10f, ForceMode.Impulse);
        StartCoroutine("Boom");
        Invoke("DestroyRPC", 1.5f);

    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Ground")||(!PV.IsMine && col.CompareTag("Player")))
        {
            rgbd.isKinematic = true;
            PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
            
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }
    }

    IEnumerator Boom()
    {
        yield return new WaitForSeconds(1.47f);
        PhotonNetwork.Instantiate("Explosion", transform.position, transform.rotation);
    }
    [PunRPC]
    void DestroyRPC()
    {
        Destroy(this.gameObject);
    }

}

