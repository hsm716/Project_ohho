using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : MonoBehaviour
{
    //public string questName;
    //public int[] npcId;
    public bool[] questIsActive = { false };    //퀘스트 진행중(각 플레이어)
    public bool[] questClearCheck = { false };  //퀘스트 클리어함(각 플레이어)(점령X)

    public bool areaReach = false;

    public void Quest()
    {
        if(questIsActive[0] == true)    //첫 번째(섹션1)의 퀘스트를 받았을 때
        {
            if (areaReach)  //QuestTrigger 스크립트 작성 > collider에 넣기
            {
                questClearCheck[0] = true;
            }
        }
    }
}
