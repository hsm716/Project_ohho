using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class QuestManager : MonoBehaviourPunCallbacks
{
    public static QuestManager Instance;
    public bool[] questOk = { false, false, false, false, false, false };   //false > �̿Ϸ���� �� ������ ����Ʈ
    public int[] SectionOwner = { 0, 0, 0, 0, 0, 0 };   //�� �÷��̾��� Photon viewID > � �÷��̾ ������ ��������

    public GameObject QuestARUI;    //����Ʈ ���� ���� UI
    public GameObject QuestClearUI;   //����Ʈ ����Ʈ UI
    public GameObject QuestBoardUI;   //����Ʈ ����Ʈ UI

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
        questText.text = dialogue.npcId + "�� ������ ����Ʈ�� �����Ͻðڽ��ϱ�?";   //dialogue�� ������ ���� ���� ���� ��
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
                p.GetComponent<QuestData>().questIsActive[dialogue.npcId] = true;    //�ش� �÷��̾�� �ش� ����Ʈ�� ���������� ��ȯ
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
    public void QuestClear2() //�ٽ� ���� �ɾ��� ���� �ӽ�
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)//�ش� �÷��̾�
            {
                if (p.GetComponent<QuestData>().questClearCheck[dialogue.npcId])    //Ŭ���� �ߴٸ�
                {
                    SectionOwner[dialogue.npcId] = p.GetComponent<PhotonView>().ViewID;   //����
                    questOk[dialogue.npcId] = true;       //���ɿ���
                }
            }
        }

        foreach (GameObject p in players)
        {
            int i = 0;
            p.GetComponent<QuestData>().questClearCheck[dialogue.npcId] = false;    //�������� ���߿� �������� �� �ش� ����Ʈ Ŭ���� ���
            p.GetComponent<QuestData>().questIsActive[dialogue.npcId] = false;   //�������� ����Ʈ ���
            i++;
        }

        QuestClearUI.SetActive(false);

    }
}
