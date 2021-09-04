
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

    string[] SoldierType_melee_str = { "Soldier_main_melee", "Soldier_main_melee_B", "Soldier_main_melee_C", "Soldier_main_melee", "Soldier_main_melee" };
    string[] SoldierType_arrow_str = { "Soldier_main_arrow", "Soldier_main_arrow_B", "Soldier_main_arrow_C", "Soldier_main_arrow" , "Soldier_main_arrow" };
    int SoldierType;

    public GameObject Respawn_Center;

    public GameManager gm;
    public Button Ready;
    public GameObject GameStart;


    bool isReady;


    public Image ranking_img;
    public TextMeshProUGUI ranking_txt;
    public TextMeshProUGUI username_txt;
    public Sprite[] ranking_123_sp;

    int soldierPoint;
    private void Awake()
    {
        FindMyPlayer();
        
        player_data.transform.position = Respawn_Center.transform.GetChild((int)(player_data.PV.ViewID / 1000) - 1).position;
        isActive_SoldierSpot = new int[,] { { 0,0,0,0,0,0,0,0,0,0},
                                             {0,0,0,0,0,0,0,0,0,0},
                                             {0,0,0,0,0,0,0,0,0,0},
                                             {0,0,0,0,0,0,0,0,0,0},
        };
    }
    // 내 클라이언트에서 조작하고 있는 플레이어를 찾는 과정
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
    public void ButtonClick()
    {
        this.transform.GetChild(2).gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
    void Update()
    {
        ranking_img.sprite = ranking_123_sp[player_data.arenaRank - 1];
        ranking_txt.text = ""+player_data.arenaRank;
        username_txt.text = player_data.username;

        curSoldierPoint.text = soldierPoint+" / " +player_data.level;

        SoldierType = player_data.SoldierType;

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

    }

    // 병사를 배치하는 과정
    public void Soldier_assign(string index)
    {
        // 준비버튼을 누르지 않았을 때만, 설정가능
        if (isReady == false)
        {
            int soldier_type = 0;

            // 배치할 병사 타입별로 type 값을 변경
            if (isSelect_melee)
            {
                soldier_type = 1;
            }
            else if (isSelect_arrow)
            {
                soldier_type = 2;
            }

            // [4,10] 배열로 되어있는 배치판에 선택한 인덱스에 선택한 타입의 병사를 배치함.
            if (isActive_SoldierSpot[index[0] - '0', index[2] - '0'] == soldier_type)
            {
                int prevType = isActive_SoldierSpot[index[0] - '0', index[2] - '0'];
                soldier_type = 0;
                isActive_SoldierSpot[index[0] - '0', index[2] - '0'] = soldier_type;
                soldierPoint += prevType;
            }
            else
            {
                int prevType = isActive_SoldierSpot[index[0] - '0', index[2] - '0'];
                isActive_SoldierSpot[index[0] - '0', index[2] - '0'] = soldier_type;
                soldierPoint += prevType - soldier_type;
            }
            // 배치한 병사 타입에 맞게 이미지를 변경시켜줌.
            SoldierSpot[index[0] - '0'].transform.GetChild(index[2] - '0').GetComponent<Image>().sprite = Soldier_type_sp[soldier_type];
        }
    }
    private void OnEnable()
    {
        PV.RPC("InitReadyCount", RpcTarget.All);
        player_data.rgbd.isKinematic = true;
        player_data.transform.position = Respawn_Center.transform.GetChild((player_data.PV.ViewID / 1000) - 1).position;
        soldierPoint = player_data.level;
        GameManager.Instance.areanaCount += 1;
    }
    public void ReadyGame()
    {
        if (isReady == false && soldierPoint >=0)
        {
            
            PV.RPC("ReadyGamePlus_RPC", RpcTarget.All);
           
            Ready.image.color = new Color(1f, 1f, 1f);
            isReady = true;
        }
        else if(isReady == true)
        {
           
            PV.RPC("ReadyGameMinus_RPC", RpcTarget.All);
           
            Ready.image.color = new Color(1f, 1f, 0.5f);
            isReady = false;
        }

       

    }
    [PunRPC]
    void InitReadyCount()
    {
        player_data.PI.gm.ReadyCountCur = 0;
    }
    [PunRPC]
    void ReadyGamePlus_RPC()
    {
        GameManager.Instance.ReadyCountCur += 1;
    }
    [PunRPC]
    void ReadyGameMinus_RPC()
    {
        GameManager.Instance.ReadyCountCur -= 1;
    }

    public void StartGame()
    {
        PV.RPC("StartGame_RPC",RpcTarget.All);
    }
    [PunRPC]
    void StartGame_RPC()
    {
        player_data.rgbd.isKinematic = false;
        GameManager.Instance.isActive = true;
        player_data.PI.isActive_Input = true;
        player_data.isArena = true;
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
        soldierPoint = player_data.level;
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

    // 병사들 생성
    void Soldier_Spawn()
    {
        for (int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                // 해당 인덱스 내, 병사 타입에 따라 결정함.
                if(isActive_SoldierSpot[i,j] == 1)
                {
                    GameObject go = PhotonNetwork.Instantiate(SoldierType_melee_str[SoldierType], player_data.transform.position, transform.rotation);
                    //go.transform.parent = player_data.transform;
                    Soldier so = go.transform.GetChild(0).GetComponent<Soldier>();
                    so.myNumber = j;
                    so.mySetNumber = i;
                }
                else if(isActive_SoldierSpot[i, j] == 2)
                {
                    GameObject go = PhotonNetwork.Instantiate(SoldierType_arrow_str[SoldierType], player_data.transform.position, transform.rotation);
                    //go.transform.parent = player_data.transform;
                    Soldier so = go.transform.GetChild(0).GetComponent<Soldier>();
                    so.myNumber = j;
                    so.mySetNumber = i;
                }
            }
        }

    }
    // Update is called once per frame

}
