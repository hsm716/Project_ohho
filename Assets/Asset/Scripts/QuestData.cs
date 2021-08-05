using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class QuestData : MonoBehaviourPunCallbacks//, IPunObservable
{
    //public string questName;
    //public int[] npcId;
    public PhotonView PV;

    public bool[] questIsActive = { false };    //����Ʈ ������(�� �÷��̾�)
    public bool[] questClearCheck = { false };  //����Ʈ Ŭ������(�� �÷��̾�)(����X)
    public GameObject QuestBoardPanel;   //����Ʈ ���� UI
    public GameObject GiveupPanel;   //����Ʈ ���� UI
    public int questnum = 0;

    public Transform QuestBoard_List;
    public GameObject[] QuestGiveUpButton;

    public bool areaReach = false;
    public int killcount = 0;

    public void Quest()
    {
        if(questIsActive[0] == true)    //ù ��°(����1)�� ����Ʈ�� �޾��� ��
        {
            if (killcount >= 5) //������ 5���� óġ
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

    public void ShowListComponent(int npcID)    //����Ʈ ������
    {
        //QuestBoard_List[npcID].SetActive(true);
        QuestBoard_List.GetChild(npcID).gameObject.SetActive(true);
    }

    public void CloseListComponent(int npcID)   //����Ʈ ����(�ٸ� ����� ���� Ŭ����)��
    {
        //QuestBoard_List[npcID].SetActive(false);
        QuestBoard_List.GetChild(npcID).gameObject.SetActive(false);
    }

    public void ShowButtonComponent(int npcID)  //����Ʈ ������
    {
        QuestGiveUpButton[npcID].SetActive(true);
    }

    public void CloseButtonComponent(int npcID) //����Ʈ Ŭ�����(�ڽ�)
    {
        QuestGiveUpButton[npcID].SetActive(false);
    }
}
