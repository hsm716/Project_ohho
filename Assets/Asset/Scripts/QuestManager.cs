using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class QuestManager : MonoBehaviourPunCallbacks//, IPunObservable
{
    public static QuestManager Instance;
    public bool[] questOk = { false, false, false, false, false };   //false > 구역이 점령되지 않은 상태
    public int[] SectionOwner = { 0, 0, 0, 0, 0, 0 };   //> 각 구역의 점령자의 viewID

    public GameObject QuestARUI;    //퀘스트 수락, 거절 UI
    public GameObject QuestClearUI;   //퀘스트 완료 UI

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

    public void ShowQuest(Dialogue _dialogue)   //퀘스트AR UI 표시
    {
        QuestARUI.SetActive(true);
        dialogue = _dialogue;
        questARText.text = dialogue.npcId + "번 구역의 퀘스트를 수락하시겠습니까?";   //각 구역별로 다르게 가능
    }

    public void ShowQuestClear(Dialogue _dialogue)  //퀘스트 완료 UI 표시
    {
        QuestClearUI.SetActive(true);
        dialogue = _dialogue;
    }

    public void AcceptQuest()  //퀘스트 수락
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)   //자신만
            {
                p.GetComponent<QuestData>().questIsActive[dialogue.npcId] = true;    //각 구역의 퀘스트를 진행중으로 전환

                p.GetComponent<QuestData>().QuestReset(dialogue.npcId);  //퀘스트 조건 리셋
                p.GetComponent<QuestData>().ShowListComponent(dialogue.npcId);   //해당 구역의 퀘스트 요소 활성화
                p.GetComponent<QuestData>().ShowButtonComponent(dialogue.npcId);   //각 구역의 퀘스트 포기버튼 활성화
            }
        }
        QuestARUI.SetActive(false);
    }

    public void GiveupQuest(int npcID)  //퀘스트 포기
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)   //자신만
            {
                p.GetComponent<QuestData>().questIsActive[npcID] = false;    //각 구역의 퀘스트를 포기
                p.GetComponent<QuestData>().questClearCheck[npcID] = false;    //완료 조건을 만족시켰더라도 포기하면 사라짐

                p.GetComponent<QuestData>().CloseListComponent(npcID);   //해당 구역의 퀘스트 요소 비활성화
            }
        }
    }

    public void RejectQuest()   //퀘스트 거절
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
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)   //자신만
            {
                if (p.GetComponent<QuestData>().questClearCheck[dialogue.npcId])
                {
                    npcID = dialogue.npcId;
                    viewID = p.GetComponent<PhotonView>().ViewID;
                    
                    p.GetComponent<QuestData>().CloseButtonComponent(npcID); //해당 퀘스트 포기 버튼 비활성화
                }
            }
        }

        QuestClearUI.SetActive(false);
        //QuestGiveUpButton[npcID].gameObject.SetActive(false);        ///////////////

        PV.RPC("QuestClear2", RpcTarget.All, npcID, viewID, ok);
    }

    [PunRPC]
    public void QuestClear2(int npcID, int viewID, bool ok)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.ViewID == viewID)
            {
                if (SectionOwner[npcID] != 0)
                {
                    p.GetComponent<Player_Control>().yaktal++;
                }
            
            }
        }

        SectionOwner[npcID] = viewID;
        questOk[npcID] = ok;

        foreach (GameObject p in players)   //다른 사람의 퀘스트 진행을 삭제
        {
            if (p.GetComponent<Player_Control>().PV.ViewID == viewID)   //자신만
            {
                p.GetComponent<QuestData>().questIsActive[npcID] = false;
                p.GetComponent<QuestData>().questClearCheck[npcID] = true;
                continue;
            }
            p.GetComponent<QuestData>().questClearCheck[npcID] = false;
            p.GetComponent<QuestData>().questIsActive[npcID] = false;
            
            //if (!(p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName))    //자신을 제외한 나머지
            //if (!(p.GetComponent<Player_Control>().PV.Owner.NickName.Equals(PhotonNetwork.LocalPlayer.NickName) ))    //자신을 제외한 나머지
            if (p.GetComponent<PhotonView>().ViewID != viewID)    //자신을 제외한 나머지
            {
                p.GetComponent<QuestData>().CloseListComponent(npcID);   //퀘스트 보드에 그 구역 퀘스트 삭제
            }
            

        }
    }

}
