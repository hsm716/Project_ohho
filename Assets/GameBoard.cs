
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    public PhotonView PV;

    public GameObject playerList;

    public Image[] classType_img;
    public Sprite swordClass_sp;
    public Sprite arrowClass_sp;
    public Sprite magicClass_sp;

    public Image[] userIcon_img;

    public TextMeshProUGUI[] KD_txt;

    public TextMeshProUGUI[] userName_txt;

    //public Image[,] occupied_Check_img;
    //public Sprite check_sp;



    int player_count;
    void Start()
    {
        player_count = PhotonNetwork.PlayerList.Count();
        for(int i = 0; i < player_count; i++)
        {
            playerList.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    void FindPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(var player in players)
        {
            Player_Control player_data_ = player.GetComponent<Player_Control>();
            int index = (player_data_.PV.ViewID / 1000) - 1;

            //공격 스타일 //
            if (player_data_.curStyle == Style.WeaponStyle.Sword)
            {
                classType_img[index].sprite = swordClass_sp;
            }
            else if (player_data_.curStyle == Style.WeaponStyle.Arrow)
            {
                classType_img[index].sprite = arrowClass_sp;
            }
            else if (player_data_.curStyle == Style.WeaponStyle.Magic)
            {
                classType_img[index].sprite = magicClass_sp;
            }
            ///////////////////////////////////////////////////

            KD_txt[index].text = player_data_.kill_point + " / " + player_data_.death_point;

            userName_txt[index].text = player_data_.username;

        }
    }
    void Update()
    {
        FindPlayers();
        if (PV.IsMine)
        {
            playerList.transform.GetChild(0);
        }
    }
}
