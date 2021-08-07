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

    public Transform QuestBoard_List;
    public GameObject[] QuestGiveUpButton;

    public int slimeKillCount = 0;
    private int slimeMaxCount = 5;

    //public bool areaReach = false;

    private void Update()
    {
        slimeSlider.value = 100 * slimeKillCount / slimeMaxCount;
        Slime.text = slimeKillCount + " / " + slimeMaxCount;
    }

    public void Quest()
    {
        if(questIsActive[0] == true)    //첫 번째(섹션1)의 퀘스트를 받았을 때
        {
            if (slimeKillCount >= slimeMaxCount) //슬라임 5마리 처치
            {
                questClearCheck[0] = true;

            }
        }
        if (questIsActive[1] == true)
        {

        }
        if (questIsActive[2] == true)
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
