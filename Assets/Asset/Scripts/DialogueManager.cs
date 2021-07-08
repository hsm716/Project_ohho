using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class DialogueManager : MonoBehaviourPunCallbacks,IPunObservable
{
    private Queue<string> sentences;

    public Text nameText;
    public Text dialoguText;
    public Button nextDialogueButton;
    public Animator animator;
    public PhotonView PV;

    public float speakDelay;

    public GameObject Wall;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;
        this.speakDelay = dialogue.speakDelay;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }
    [PunRPC]
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            Wall.GetComponent<StartWall>().WallDown();
            return;
        }

        nextDialogueButton.interactable = false;
        string sentence = sentences.Dequeue();
        dialoguText.text = sentence;
        nextDialogueButton.interactable = true;
        //StopAllCoroutines();
        //StartCoroutine(TypeSentence(sentence, speakDelay));
    }

    IEnumerator TypeSentence(string sentence, float delay)
    {
        dialoguText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialoguText.text += letter;
            yield return new WaitForSeconds(delay);
            yield return null;
        }

        nextDialogueButton.interactable = true;
    }

    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(dialoguText.text);
        }
        else
        {
            dialoguText.text = (string)stream.ReceiveNext();
        }
    }
}