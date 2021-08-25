using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DialogueManager : MonoBehaviourPunCallbacks
{
    private Queue<string> sentences;

    public GameObject dialoguePanel;
    public Text nameText;
    public Text dialogueText;
    public Button nextDialogueButton;
    public float speakDelay;

    public Dialogue dialogue;

    void Start()
    {
        sentences = new Queue<string>();
    }

    GameObject FindMyPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                return p;
            }
        }
        return null;
    }

    public void StartDialogue(Dialogue _dialogue, bool isComplete)
    {
        dialogue = _dialogue;
        //animator.SetBool("IsOpen", true);
        dialoguePanel.SetActive(true);

        nameText.text = dialogue.name;
        this.speakDelay = dialogue.speakDelay;

        sentences.Clear();

        GameObject myPlayer = FindMyPlayer();

        if (!isComplete) //클리어가 안된 상태라면
        {
            if (QuestManager.Instance.SectionOwner[dialogue.npcId] == myPlayer.GetComponent<PhotonView>().ViewID)    //이미 그 지역의 주인이라면
            {
                foreach (string sentence in dialogue.AlreadySentences)
                {
                    sentences.Enqueue(sentence);
                }
            }
            else if (!myPlayer.GetComponent<QuestData>().questIsActive[dialogue.npcId])   //진행중이 아닐때
            {
                foreach (string sentence in dialogue.unCompletedSentences)//클러어 안됐을 때 대화
                {
                    sentences.Enqueue(sentence);
                }
            }
            else    //플레이어가 이미 해당 퀘스트를 진행중이라면
            {   //내가 진행중일 때 or 남이 진행중일 때 2가지 ex
                foreach (string sentence in dialogue.duringSentences)//클러어 됐을 때 대화
                {
                    sentences.Enqueue(sentence);
                }
            }

        }
        else            //클리어 상태라면  >> 내가 점령했을 때 or 남이 점령했을 때 2가지   //주인이 없거나 다른 플레이어의 구역인 상태의 퀘스트를 클리어 했을 때
        {
         
            foreach (string sentence in dialogue.CompletedSentences)
            {
                sentences.Enqueue(sentence);
            }
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        GameObject myPlayer = FindMyPlayer();

        if (sentences.Count == 0)   //마지막 대화가 끝났을 때
        {
            if (QuestManager.Instance.SectionOwner[dialogue.npcId] == myPlayer.GetComponent<PhotonView>().ViewID)   //이미 자신의 구역일때
            {
                EndDialogue();
            }
            else if (!myPlayer.GetComponent<QuestData>().questIsActive[dialogue.npcId])//퀘스트가 진행중이 아닐 때
            {
                QuestUI(dialogue);  //퀘스트 수락 or 거절 UI 생성
            }
            else //퀘스트가 진행 중일 때
            {
                if (!myPlayer.GetComponent<QuestData>().questClearCheck[dialogue.npcId]) //퀘스트를 아직 클리어 하지 못했을 때
                {
                    EndDialogue();
                }
                else        //퀘스트를 클리어 했을 때
                {
                    QuestClearUI(dialogue); //퀘스트 완료 UI 생성
                }
                
            }
            
            return;
        }

        nextDialogueButton.interactable = false;
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, speakDelay));
    }

    IEnumerator TypeSentence(string sentence, float delay)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(delay);
            yield return null;
        }

        nextDialogueButton.interactable = true;
    }

    public void EndDialogue()
    {
        //animator.SetBool("IsOpen", false);
        dialoguePanel.SetActive(false);
    }

    public void QuestUI(Dialogue dialogue)
    {
        //animator.SetBool("IsOpen", false);
        dialoguePanel.SetActive(false);
        QuestManager.Instance.ShowQuest(dialogue);
    }

    public void QuestClearUI(Dialogue dialogue)
    {
        //animator.SetBool("IsOpen", false);
        dialoguePanel.SetActive(false);
        QuestManager.Instance.ShowQuestClear(dialogue);
    }

    public void StartDialogue(Dialogue _dialogue)
    {
        dialogue = _dialogue;
        dialoguePanel.SetActive(true);

        nameText.text = _dialogue.name;
        this.speakDelay = _dialogue.speakDelay;

        sentences.Clear();

        GameObject myPlayer = FindMyPlayer();

        foreach (string sentence in dialogue.unCompletedSentences)
        {
            sentences.Enqueue(sentence);
        }

        NextSentence();
    }

    public void NextSentence()
    {
        GameObject myPlayer = FindMyPlayer();

        if (sentences.Count == 0)   //마지막 대화가 끝났을 때
        {
            EndDialogue();
            RewardManager.Instance.FadeInTest();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeFinal(sentence, speakDelay));
    }

    IEnumerator TypeFinal(string sentence, float delay)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(delay);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);
        NextSentence();
    }
}