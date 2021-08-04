using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class QuestData : MonoBehaviourPunCallbacks//, IPunObservable
{
    //public string questName;
    //public int[] npcId;
    public PhotonView PV;

    public bool[] questIsActive = { false };    //퀘스트 진행중(각 플레이어)
    public bool[] questClearCheck = { false };  //퀘스트 클리어함(각 플레이어)(점령X)

    public bool areaReach = false;
    public int killcount = 0;


    public void Quest()
    {
        if(questIsActive[0] == true)    //첫 번째(섹션1)의 퀘스트를 받았을 때
        {
            if (killcount >= 5) //슬라임 5마리 처치
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
