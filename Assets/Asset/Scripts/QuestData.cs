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

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(questIsActive);
            stream.SendNext(questClearCheck);
        }
        else
        {
            questIsActive = (bool[])stream.ReceiveNext();
            questClearCheck = (bool[])stream.ReceiveNext();
        }
    }
    */
}
