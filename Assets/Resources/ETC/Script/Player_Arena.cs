
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Arena : MonoBehaviour
{
    public Player_Control player_data;

    public Image button_melee;
    public Image button_arrow;

    bool isSelect_melee;
    bool isSelect_arrow;
    public int[,] isActive_SoldierSpot;
    public GameObject[] SoldierSpot;
    public Sprite[] Soldier_type_sp;

    public TextMeshProUGUI curSoldierPoint;

    string[] SoldierType_melee_str = { "Soldier_main_melee", "Soldier_main_melee_B", "Soldier_main_melee_C" };
    string[] SoldierType_arrow_str = { "Soldier_main_arrow", "Soldier_main_arrow_B", "Soldier_main_arrow_C" };
    int SoldierType;
    private void Awake()
    {
        SoldierType = player_data.SoldierType;
        isActive_SoldierSpot = new int[,] { { 0,0,0,0,0,0,0,0,0,0},
                                             {0,0,0,0,0,0,0,0,0,0},
                                             {0,0,0,0,0,0,0,0,0,0},
                                             {0,0,0,0,0,0,0,0,0,0},
        };
    }


    void Update()
    {

        curSoldierPoint.text = player_data.SoldierPoint+" / " +player_data.SoldierPoint_max;
        if (isSelect_arrow)
        {
            button_arrow.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }
        else
            button_arrow.transform.localScale = new Vector3(1f, 1f, 1f);

        if (isSelect_melee)
        {
            button_melee.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }
        else
            button_melee.transform.localScale = new Vector3(1f, 1f, 1f);

    }
    public void Soldier_assign(string index)
    {
        int soldier_type = 0;
        if (isSelect_melee)
        {
            soldier_type = 1;
        }
        else if (isSelect_arrow)
        {
            soldier_type = 2;
        }
        if (isActive_SoldierSpot[index[0] - '0', index[2] - '0'] == soldier_type)
        {
            int prevType = isActive_SoldierSpot[index[0] - '0', index[2] - '0'];
            soldier_type = 0;
            isActive_SoldierSpot[index[0] - '0', index[2] - '0'] = soldier_type;
            player_data.SoldierPoint += prevType;
        }
        else
        {
            int prevType = isActive_SoldierSpot[index[0] - '0', index[2] - '0'];
            isActive_SoldierSpot[index[0] - '0', index[2] - '0'] = soldier_type;
            player_data.SoldierPoint += prevType - soldier_type;
        }

        SoldierSpot[index[0] - '0'].transform.GetChild(index[2] - '0').GetComponent<Image>().sprite = Soldier_type_sp[soldier_type];



    }
    public void StartGame()
    {
        Soldier_Spawn();
        player_data.PI.time = 60f;
        player_data.PI.isArena = false;
        Invoke("active_false", 1f);
    }
    void active_false()
    {
        this.gameObject.SetActive(false);
    }

    public void Select_melee()
    {
        isSelect_melee = (isSelect_melee == true ? false : true);
        isSelect_arrow = false;
    }
    public void Select_arrow()
    {
        isSelect_melee = false;
        isSelect_arrow = (isSelect_arrow == true ? false : true);
    }
    void Soldier_Spawn()
    {
        for (int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                if(isActive_SoldierSpot[i,j] == 1)
                {
                    GameObject go = PhotonNetwork.Instantiate(SoldierType_melee_str[SoldierType], player_data.transform.position, transform.rotation);
                    Soldier so = go.transform.GetChild(0).GetComponent<Soldier>();
                    so.myNumber = j;
                    so.mySetNumber = i;
                }
                else if(isActive_SoldierSpot[i, j] == 2)
                {
                    GameObject go = PhotonNetwork.Instantiate(SoldierType_arrow_str[SoldierType], player_data.transform.position, transform.rotation);
                    Soldier so = go.transform.GetChild(0).GetComponent<Soldier>();
                    so.myNumber = j;
                    so.mySetNumber = i;
                }
            }
        }

    }
    // Update is called once per frame

}
