using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Style : MonoBehaviourPunCallbacks
{
    public enum WeaponStyle { None,Sword, Arrow, Magic };
    public WeaponStyle selectStyle;
    public PhotonView PV;
    public GameObject SelectPanel;


    Player_Control FindMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach( GameObject p in players)
        {
            if( p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                return p.GetComponent<Player_Control>();
            } 
        }
        return null;
    }

    public void Select_Sword()
    {
        Player_Control myPlayer = FindMyPlayer();
        myPlayer.curStyle = WeaponStyle.Sword;
        //myPlayer.WeaponPosition_R.transform.GetChild(1).gameObject.SetActive(true);
        //myPlayer.WeaponPosition_L.transform.GetChild(1).gameObject.SetActive(true);
        myPlayer.atk = 200f;
        myPlayer.animator.Rebind();
        myPlayer.animator.Play("Idle_Sword");

        SelectPanel.SetActive(false);
    }

    public void Select_Arrow()
    {
        Player_Control myPlayer = FindMyPlayer();
        myPlayer.curStyle = WeaponStyle.Arrow;
       // myPlayer.WeaponPosition_L.transform.GetChild(2).gameObject.SetActive(true);
        myPlayer.atk = 150f;
        myPlayer.animator.Rebind();
        myPlayer.animator.Play("Idle_Arrow");

        SelectPanel.SetActive(false);
    }

 
    public void Select_Magic()
    {
        Player_Control myPlayer = FindMyPlayer();
        myPlayer.curStyle = WeaponStyle.Magic;
        //myPlayer.WeaponPosition_R.transform.GetChild(2).gameObject.SetActive(true);
        myPlayer.atk = 225f;
        myPlayer.animator.Rebind();
        myPlayer.animator.Play("Idle_Magic");

        SelectPanel.SetActive(false);

    }
}
