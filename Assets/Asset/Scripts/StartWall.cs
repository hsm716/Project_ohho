using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWall : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    public Animator anim;

    [PunRPC]
    public void WallDown()
    {
        anim.SetTrigger("doDO");
    }

}
