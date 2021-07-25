using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public int npcId;   //==sectionID
    public string name;
    public float speakDelay;

    [TextArea(3,10)]
    public string[] unCompletedSentences;

    [TextArea(3, 10)]
    public string[] duringSentences;

    [TextArea(3, 10)]
    public string[] CompletedSentences;

    [TextArea(3, 10)]
    public string[] AlreadySentences;
    //2개 추가 남이 진행중일때, 남이 클리어했을때
}
