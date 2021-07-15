using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;

    public Text nameText;
    public Text dialoguText;
    public Button nextDialogueButton;
    public Animator animator;

    public float speakDelay;

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

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            //Wall.GetComponent<StartWall>().WallDown();
            return;
        }

        nextDialogueButton.interactable = false;
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, speakDelay));
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

}