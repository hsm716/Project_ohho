using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (other.GetComponent<QuestData>().questIsActive[0])   //ù ��° ����Ʈ�� Ȱ��ȭ ���� ��
            {
                other.GetComponent<QuestData>().questClearCheck[0] = true;
            }
            if (other.GetComponent<QuestData>().questIsActive[1])   //�� ��° ����Ʈ�� Ȱ��ȭ ���� ��
            {
                other.GetComponent<QuestData>().questClearCheck[1] = true;
            }
            if (other.GetComponent<QuestData>().questIsActive[2])   //�� ��° ����Ʈ�� Ȱ��ȭ ���� ��
            {
                other.GetComponent<QuestData>().questClearCheck[2] = true;
            }
        }
    }
}
