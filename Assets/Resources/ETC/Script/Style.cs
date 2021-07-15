using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Style : MonoBehaviour
{
    public enum WeaponStyle { None,Sword, Arrow, Magic };
    public WeaponStyle selectStyle;
    public PhotonView PV;
    public Player_Control myPlayer;

    public GameObject SelectPanel;

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

    public void Select_Sword()
    {
        FindMyPlayer();
        myPlayer.curStyle = WeaponStyle.Sword;
        myPlayer.WeaponPosition_R.transform.GetChild(1).gameObject.SetActive(true);
        myPlayer.WeaponPosition_L.transform.GetChild(1).gameObject.SetActive(true);
        myPlayer.atk = 200f;
        myPlayer.animator.Rebind();
        myPlayer.animator.Play("Idle_Sword");

        SelectPanel.SetActive(false);
    }

    public void Select_Arrow()
    {
        FindMyPlayer();
        myPlayer.curStyle = WeaponStyle.Arrow;
        myPlayer.WeaponPosition_L.transform.GetChild(2).gameObject.SetActive(true);
        myPlayer.atk = 150f;
        myPlayer.animator.Rebind();
        myPlayer.animator.Play("Idle_Arrow");

        SelectPanel.SetActive(false);
    }

    public void Select_Magic()
    {
        FindMyPlayer();
        myPlayer.curStyle = WeaponStyle.Magic;
        myPlayer.WeaponPosition_R.transform.GetChild(2).gameObject.SetActive(true);
        myPlayer.atk = 175f;
        myPlayer.animator.Rebind();
        myPlayer.animator.Play("Idle_Magic");

        SelectPanel.SetActive(false);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            switch (selectStyle)
            {
                case WeaponStyle.Sword:

                    break;
                case WeaponStyle.Arrow:


                    break;
                case WeaponStyle.Magic:

                    break;

            }
        }
    }
}
