using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class hpBar_ob : MonoBehaviour
{
    public GameObject player;
  //  public PhotonView PV;

 //   private Player_Control playerLogic;
    Vector3 offset= new Vector3(0f,2.8f,0f);

    private void Awake()
    {
       // playerLogic = player.GetComponent<Player_Control>();
    }

    void Update()
    {
        
     /*   if (Input.GetKeyDown(KeyCode.Escape))
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);*/
        transform.position = player.transform.position + offset;
    
    }
    
   // void DestoryRPC() => Destroy(gameObject);
}
