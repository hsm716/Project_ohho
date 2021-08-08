using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (other.GetComponent<QuestData>().questIsActive[0])   //첫 번째 퀘스트가 활성화 중일 때
            {
                other.GetComponent<QuestData>().questClearCheck[0] = true;
            }
            if (other.GetComponent<QuestData>().questIsActive[1])   //두 번째 퀘스트가 활성화 중일 때
            {
                other.GetComponent<QuestData>().questClearCheck[1] = true;
            }
            if (other.GetComponent<QuestData>().questIsActive[2])   //두 번째 퀘스트가 활성화 중일 때
            {
                other.GetComponent<QuestData>().questClearCheck[2] = true;
            }
        }
    }
}
