using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class QuestManager : MonoBehaviourPunCallbacks//, IPunObservable
{
    public static QuestManager Instance;
    public bool[] questOk = { false, false, false, false, false, false };   //false > 구역이 점령되지 않은 상태
    public int[] SectionOwner = { 0, 0, 0, 0, 0, 0 };   //> 각 구역의 점령자의 viewID

    public GameObject QuestARUI;    //퀘스트 수락, 거절 UI
    public GameObject QuestClearUI;   //퀘스트 완료 UI
    public GameObject QuestBoardUI;   //퀘스트 보드 UI

    public Dialogue dialogue;
    public Text questARText;

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
        questARText.text = dialogue.npcId + "퀘스트를 수락하시겠습니까?";   //각 구역별로 다르게 가능
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

    public void AcceptQuest()  //����Ʈ ����
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                p.GetComponent<QuestData>().questIsActive[dialogue.npcId] = true;    //각 구역의 퀘스트를 진행중으로 전환
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
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int npcID = 0, viewID = 0;
        bool ok = true;

        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                if (p.GetComponent<QuestData>().questClearCheck[dialogue.npcId])
                {
                    npcID = dialogue.npcId;
                    viewID = p.GetComponent<PhotonView>().ViewID;
                }
            }
        }

        QuestClearUI.SetActive(false);

        PV.RPC("QuestClear2", RpcTarget.All, npcID, viewID, ok);
    }

    [PunRPC]
    public void QuestClear2(int npcID, int viewID, bool ok)
    {
        SectionOwner[npcID] = viewID;
        questOk[npcID] = ok;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)   //다른 사람의 퀘스트 진행을 삭제
        {
            p.GetComponent<QuestData>().questClearCheck[npcID] = false;
            p.GetComponent<QuestData>().questIsActive[npcID] = false;
        }
    }

}
