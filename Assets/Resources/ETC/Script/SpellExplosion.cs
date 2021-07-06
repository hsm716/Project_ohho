using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellExplosion : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    void Awake()
    {
        Invoke("DestroyRPC", 1.5f);
    }
    [PunRPC]
    void DestroyRPC() => Destroy(this.gameObject);
}
