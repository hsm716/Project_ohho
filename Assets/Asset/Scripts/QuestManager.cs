using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class QuestManager : MonoBehaviourPunCallbacks
{
    public static QuestManager Instance;
    public bool[] questOk = { false, false, false, false, false, false };   //false > 미완료상태 각 섹션의 퀘스트
    public int[] SectionOwner = { 0, 0, 0, 0, 0, 0 };   //각 플레이어의 Photon viewID > 어떤 플레이어가 섹션의 주인인지

    public GameObject QuestARUI;    //퀘스트 수락 거절 UI
    public GameObject QuestClearUI;   //퀘스트 리스트 UI
    public GameObject QuestBoardUI;   //퀘스트 리스트 UI

    public Dialogue dialogue;
    public Text questText;

    public PhotonView PV;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }


    public bool QuestClearCheck(int npcId)
    {
        return questOk[npcId];
    }

    public void ShowQuest(Dialogue _dialogue)
    {
        QuestARUI.SetActive(true);
        dialogue = _dialogue;
        questText.text = dialogue.npcId + "번 구역의 퀘스트를 수락하시겠습니까?";   //dialogue에 수락용 문장 따로 만들어도 됌
    }

    public void ShowQuestClear(Dialogue _dialogue)
    {
        QuestClearUI.SetActive(true);
        dialogue = _dialogue;
    }

    public void ShowQuestBoard()
    {
        QuestBoardUI.SetActive(true);
    }

    public void CloseQuestBoard()
    {
        QuestBoardUI.SetActive(false);
    }

    public void AcceptQuest()  //퀘스트 수락
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                p.GetComponent<QuestData>().questIsActive[dialogue.npcId] = true;    //해당 플레이어에게 해당 퀘스트를 진행중으로 전환
            }
        }
        QuestARUI.SetActive(false);
    }

    public void RejectQuest()
    {
        QuestARUI.SetActive(false);
    }

    public void QuestClear()
    {
        PV.RPC("QuestClear2", RpcTarget.All);
    }

    [PunRPC]
    public void QuestClear2() //다시 말을 걸었을 때로 임시
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)//해당 플레이어
            {
                if (p.GetComponent<QuestData>().questClearCheck[dialogue.npcId])    //클리어 했다면
                {
                    SectionOwner[dialogue.npcId] = p.GetComponent<PhotonView>().ViewID;   //주인
                    questOk[dialogue.npcId] = true;       //점령여부
                }
            }
        }

        foreach (GameObject p in players)
        {
            int i = 0;
            p.GetComponent<QuestData>().questClearCheck[dialogue.npcId] = false;    //누군가가 나중에 점령했을 때 해당 퀘스트 클리어 취소
            p.GetComponent<QuestData>().questIsActive[dialogue.npcId] = false;   //진행중인 퀘스트 취소
            i++;
        }

        QuestClearUI.SetActive(false);

    }
}
