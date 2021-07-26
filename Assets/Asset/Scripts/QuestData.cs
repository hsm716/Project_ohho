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

    public void Quest()
    {
        if(questIsActive[0] == true)    //ù ��°(����1)�� ����Ʈ�� �޾��� ��
        {
            if (areaReach)  //QuestTrigger ��ũ��Ʈ �ۼ� > collider�� �ֱ�
            {
                questClearCheck[0] = true;
            }
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
