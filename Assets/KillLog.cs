using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillLog : MonoBehaviour
{
    public PhotonView PV;

    public Image Killer_Icon;
    public TextMeshProUGUI killer_name;

    Animator anim;
    public TextMeshProUGUI killedMan_name;
    public Image KilledMan_Icon;

    public Player_Control myPlayer;
    public GameObject KillLogs;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        KillLogs = GameObject.Find("KillLogs");
        FindMyPlayer();


    }
    [PunRPC]
    void SetLog_RPC()
    {
        transform.parent = KillLogs.transform;
        if (!myPlayer.Last_Hiter)
        {
            killer_name.text = "MONSTER";
            killedMan_name.text = myPlayer.username;
        }
        else
        {
            killer_name.text = myPlayer.Last_Hiter.username;
            killedMan_name.text = myPlayer.username;
        }

        
    }
    void Start()
    {
        PV.RPC("SetLog_RPC", RpcTarget.All);
        Invoke("DoOff", 3f);
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
    public void DoOff()
    {
        anim.SetTrigger("doOff");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void DeleteLog()
    {
        PV.RPC("DestoryRPC", RpcTarget.All);
    }

    [PunRPC]
    void DestroyRPC()
    {
        Destroy(this.gameObject);
    }
}
