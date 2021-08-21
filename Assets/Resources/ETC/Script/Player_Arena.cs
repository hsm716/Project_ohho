
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
    public PhotonView PV;
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

    public GameObject Respawn_Center;

    public GameManager gm;
    public Button Ready;
    public GameObject GameStart;


    bool isReady;

    bool isSetRank;

    public Image ranking_img;
    public Sprite[] ranking_123_sp;

    public int rank;
    
    private void Awake()
    {
        FindMyPlayer();
        SoldierType = player_data.SoldierType;
        player_data.transform.position = Respawn_Center.transform.GetChild((player_data.PV.ViewID / 1000) - 1).position;
        isActive_SoldierSpot = new int[,] { { 0,0,0,0,0,0,0,0,0,0},
                                             {0,0,0,0,0,0,0,0,0,0},
                                             {0,0,0,0,0,0,0,0,0,0},
                                             {0,0,0,0,0,0,0,0,0,0},
        };
    }
    void FindMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                player_data = p.GetComponent<Player_Control>();
                break;
            }
        }

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

        if (PhotonNetwork.IsMasterClient)
        {
            if(player_data.PI.gm.ReadyCountCur == player_data.PI.gm.ReadyCountMax)
            {
                GameStart.SetActive(true);
            }
            else
            {
                GameStart.SetActive(false);
            }
        }
        if (isSetRank==false && player_data.PI.isArena && player_data.curHP <= 0f)
        {
            isSetRank = true;
            rank = gm.ArenaRank;
            PV.RPC("SetRank", RpcTarget.All);
            gm.ArenaRank-=1;
        }

    }
    [PunRPC]
    void SetRank()
    {
        rank = gm.ArenaRank;
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
    public void ReadyGame()
    {
        if (isReady == false)
        {
            player_data.rgbd.isKinematic = true;
            player_data.transform.position = Respawn_Center.transform.GetChild((player_data.PV.ViewID / 1000) - 1).position;
            PV.RPC("ReadyGamePlus_RPC", RpcTarget.All);
           
            Ready.image.color = new Color(1f, 1f, 1f);
            isReady = true;
        }
        else
        {
            player_data.rgbd.isKinematic = true;
            player_data.transform.position = Respawn_Center.transform.GetChild((player_data.PV.ViewID / 1000) - 1).position;
            PV.RPC("ReadyGameMinus_RPC", RpcTarget.All);
           
            Ready.image.color = new Color(1f, 1f, 0.5f);
            isReady = false;
        }

       

    }
    [PunRPC]
    void ReadyGamePlus_RPC()
    {
        gm.ReadyCountCur += 1;
    }
    [PunRPC]
    void ReadyGameMinus_RPC()
    {
        gm.ReadyCountCur -= 1;
    }

    public void StartGame()
    {

        player_data.rgbd.isKinematic = false;
        PV.RPC("StartGame_RPC",RpcTarget.All);
    }
    [PunRPC]
    void StartGame_RPC()
    {
        GameManager.Instance.isActive = true;
        player_data.PI.isActive_Input = true;
        player_data.PI.isArena = true;
        player_data.PI.isArena_in = true;
        player_data.PI.MC.transform.GetChild(7).transform.GetChild(0).gameObject.SetActive(true);
        gm.arena_time = 60f;
        Soldier_Spawn();
        Invoke("active_false", 1f);
    }
    
    void active_false()
    {
        player_data.PI.MC.transform.GetChild(7).transform.GetChild(0).gameObject.SetActive(false);
        player_data.PI.MC.transform.GetChild(7).transform.GetChild(1).gameObject.SetActive(true);

        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                isActive_SoldierSpot[i, j] = 0;
            }
        }
        player_data.SoldierPoint = 20;
        isSelect_arrow = false;
        isSelect_melee = false;
        isReady = false;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                SoldierSpot[i].transform.GetChild(j).GetComponent<Image>().sprite = Soldier_type_sp[0];
            }
        }
        //this.transform.GetChild(0).gameObject.SetActive(false);
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
