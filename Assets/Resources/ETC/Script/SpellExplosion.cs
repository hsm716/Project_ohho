using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellExplosion : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public float atk;
    void Awake()
    {
        Invoke("DestroyRPC", 1.5f);
    }
    private void OnTriggerEnter(Collider col)
    {
        if ( (!PV.IsMine && col.CompareTag("Player")))
        {
           
            //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
            col.GetComponent<Player_Control>().Hit(atk);

          
        }
    }
    [PunRPC]
    void DestroyRPC() => Destroy(this.gameObject);
}
