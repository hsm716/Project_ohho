using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (RewardManager.Instance.End)
        {
            RewardManager.Instance.End = false;
            FinalDialogue();

        }*/
        if (other.tag == "Player" && other.GetComponent<Player_Control>().PV.IsMine)
        {
            //TriggerDialogue(QuestManager.Instance.QuestClearCheck(dialogue.npcId));   //퀘스트 클리어 여부에 따라 대화 생성
            TriggerDialogue(other.GetComponent<QuestData>().questClearCheck[dialogue.npcId]);   //퀘스트 클리어 여부에 따라 대화 생성
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<Player_Control>().PV.IsMine)
        {
            FindObjectOfType<DialogueManager>().EndDialogue();
        }
    }

    public void TriggerDialogue(bool isComplete)
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, isComplete);
    }

    public void FinalDialogue()
    {
        FindObjectOfType<DialogueManager>().FinalStartDialogue(dialogue);
        //GameObject.Find("DialogueManager").GetComponent<DialogueManager>().StartDialogue(dialogue);
    }
}
