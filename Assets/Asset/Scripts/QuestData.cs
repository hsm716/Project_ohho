using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : MonoBehaviour
{
    //public string questName;
    //public int[] npcId;
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
}
