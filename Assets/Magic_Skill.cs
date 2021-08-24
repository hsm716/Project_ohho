using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Magic_Skill : MonoBehaviour
{
    public float atk;
    public PhotonView PV;
    public Player_Control myPlayer;

    private void OnParticleCollision(GameObject col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<Player_Control>().Hit(atk / 10, 2, myPlayer.curCritical);
            //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));

            col.GetComponent<Player_Control>().Last_Hiter = myPlayer;

        }
        if (col.CompareTag("Soldier") && col.GetComponent<PhotonView>().Owner != PV.Owner)
        {

            //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
            col.GetComponent<Soldier>().Hit(atk / 10, 2, myPlayer.curCritical);


        }
        if (col.CompareTag("Monster"))
        {

            //PhotonNetwork.Instantiate("Explosion", transform.position+new Vector3(0f,0.3f,0f), Quaternion.Euler(new Vector3(-transform.rotation.x,-transform.rotation.y,-transform.rotation.z)));
            col.GetComponent<Monster>().Last_Hiter = myPlayer;
            col.GetComponent<Monster>().Hit(atk / 10, 2, myPlayer.curCritical);

            //PV.RPC("Set_LastHiter", RpcTarget.All, col);


        }
    }

    // Update is called once per frame
    void Update()
    {
        atk = myPlayer.atk;
    }
}
