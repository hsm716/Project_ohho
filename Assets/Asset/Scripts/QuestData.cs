using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class QuestData : MonoBehaviourPunCallbacks//, IPunObservable
{
    //public string questName;
    //public int[] npcId;
    public PhotonView PV;

    public bool[] questIsActive = { false };    //퀘스트 진행중(각 플레이어)
    public bool[] questClearCheck = { false };  //퀘스트 클리어함(각 플레이어)(점령X)
    public GameObject QuestBoardPanel;   //퀘스트 보드 UI
    public GameObject GiveupPanel;   //퀘스트 보드 UI
    public int questnum = 0;

    public Slider slimeSlider;
    public Text Slime;
    public Slider twigSlider;
    public Text twig;
    public Slider OccupiedSlider;
    public Text Occupy;

    public Transform QuestBoard_List;
    public GameObject[] QuestGiveUpButton;

    public int slimeKillCount = 0;
    private int slimeMaxCount = 5;
    public int forestSpirit_CurCount = 0;
    private int forestSpirit_MaxCount = 5;

    public bool DemonKill = false;

    public int twigCurCount = 0;
    private int twigMaxCount = 5;

    public float OccupiedValue_cur = 0f;
    public float OccupiedValue_max = 100f;



    //public bool areaReach = false;
    Player_Control myPlayer;
    private void Awake()
    {
        myPlayer = transform.GetComponent<Player_Control>();
    }
    private void Update()
    {
        if (OccupiedValue_cur >= OccupiedValue_max)
            OccupiedValue_cur = OccupiedValue_max;

        forestSpirit_CurCount = myPlayer.PI.item_Material["forest_spirit"];
        twigCurCount = myPlayer.PI.item_Material["twig"];
        OccupiedValue_cur = myPlayer.curOccupied_value;
        Quest();

        /*        slimeSlider.value = 100 * slimeKillCount / slimeMaxCount;
                Slime.text = slimeKillCount + " / " + slimeMaxCount;*/
        slimeSlider.value = 100 * forestSpirit_CurCount / forestSpirit_MaxCount;
        Slime.text = forestSpirit_CurCount + " / " + forestSpirit_MaxCount;

        twigSlider.value = (twigCurCount / (float)twigMaxCount);
        twig.text = twigCurCount + " / " + twigMaxCount;

        OccupiedSlider.value = (OccupiedValue_cur / OccupiedValue_max);
        Occupy.text = string.Format("{0:0.##}", 100*OccupiedValue_cur/OccupiedValue_max)+"%"; 
    }
/*    int FindItemNum(string name)
    {
        int num = 0;
        for (int i = 0; i < 4; i++)
        {
            if (myPlayer.Inventory_item_is[i] == true)
            {

                if (name == myPlayer.PI.Inventory_item_name[i])
                {
                    num = myPlayer.PI.Inventory_item_num[i];
                    break;
                }
            }
        }
        return num;
    }*/
    public void Quest()
    {
        if(questIsActive[0] == true)    //포레스트
        {
            /*if (slimeKillCount >= slimeMaxCount) //슬라임 5마리 처치
            {
                questClearCheck[0] = true;

            }*/
            if ( forestSpirit_CurCount >= forestSpirit_MaxCount) //슬라임 5마리 처치
            {
                questClearCheck[0] = true;

            }
        }
        if (questIsActive[1] == true)   //사막
        {
            if (twigCurCount >= twigMaxCount) // 나뭇가지 5개 주워오기
            {
                questClearCheck[1] = true;

            }
        }
        if (questIsActive[2] == true)   //헬
        {
            if (DemonKill == true)
            {
                questClearCheck[2] = true;
            }
        
        }
        if (questIsActive[3] == true)   //포지
        {
            //OccupiedValue_cur += Time.deltaTime*5f;
            if (OccupiedValue_cur >= OccupiedValue_max) // 나뭇가지 5개 주워오기
            {
                questClearCheck[3] = true;

            }
        }
        if (questIsActive[4] == true)   //설원
        {

        }

    }


    public void ShowGiveupPanel(int num)
    {
        GiveupPanel.SetActive(true);
        questnum = num;
    }

    public void CloseGiveupPanel()
    {
        GiveupPanel.SetActive(false);
    }

    public void Sure_Giveup()
    {
        GiveupPanel.SetActive(false);
        QuestManager.Instance.GiveupQuest(questnum);
    }

    public void ShowQuestBoard()
    {
        QuestBoardPanel.SetActive(true);
    }

    public void CloseQuestBoard()
    {
        QuestBoardPanel.SetActive(false);
    }

    public void ShowListComponent(int npcID)    //퀘스트 수락시
    {
        //QuestBoard_List[npcID].SetActive(true);
        QuestBoard_List.GetChild(npcID).gameObject.SetActive(true);
    }

    public void CloseListComponent(int npcID)   //퀘스트 실패(다른 사람이 먼저 클리어)시
    {
        //QuestBoard_List[npcID].SetActive(false);
        QuestBoard_List.GetChild(npcID).gameObject.SetActive(false);
    }

    public void ShowButtonComponent(int npcID)  //퀘스트 수락시
    {
        QuestGiveUpButton[npcID].SetActive(true);
    }

    public void CloseButtonComponent(int npcID) //퀘스트 클리어시(자신)
    {
        QuestGiveUpButton[npcID].SetActive(false);
    }

    public void QuestReset(int npcID)
    {
        switch (npcID)
        {
            case 0:
                slimeKillCount = 0;
                break;
            case 1:
                break;
            case 2:
                twigCurCount = 0;
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
        }
    }
}
