using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RewardManager : MonoBehaviourPunCallbacks
{
    public static RewardManager Instance;

    public SceneFader sceneFader;

    public int most_jumlyung;   //최대 점령 수
    public int most_yaktal;     //최대 약탈 수


    public GameObject Occupier;    //가장 많이 점령한 자
    public GameObject Plunderer;    //가장 많이 약탈한 자
    public GameObject Killer;    //가장 많이 킬한 자
    public GameObject Leveler;    //가장 많이 킬한 자
    public GameObject BossKiller;    //가장 많이 킬한 자

    public GameObject Final_Winner;

    public int[] arena = { 0, 0, 0 };


    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }


    public void Start()
    {
        most_jumlyung = 0;
        most_yaktal = 0;

        //tartCoroutine(Test());
    }

    IEnumerator Test()  //페이드아웃 테스트
    {
        yield return new WaitForSeconds(5f);
        Reward();
    }

    public void Tochair()   //의자있는화면으로
    {
        StartCoroutine(Test3());
    }

    IEnumerator Test3() //페이드인 테스트
    {
        yield return new WaitForSeconds(2f);
        sceneFader.FadeIn();
    }

    public void Reward()
    {
        sceneFader.FadeOut();


        Occupy();   //점령
        Plunder();  //약탈
        Arena();
        Kill();
        Level();
        BossKill();
        Final_Reward();
    }
    /*
    void UIClose()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            p.
        }
    }
    */
    void Occupy()
    {
        GameObject QM = GameObject.Find("QuestManager");

        int[] sectionOwner = { 0, 0, 0, 0, 0 };
        for (int i = 0; i < 5; i++)
        {
            sectionOwner[i] = QM.GetComponent<QuestManager>().SectionOwner[i] / 1000;   //각 구역의 주인의 viewID를 1000으로 나눈값
        }

        int[] index = { 0, 0, 0, 0 };   //3명 3까지
        for (int i = 0; i < index.Length; i++)
        {
            index[sectionOwner[i]]++;
        }

        int max = 0;
        int mode = 0;
        for (int i = 1; i < index.Length; i++)  //1부터 > 플레이어 / 1000의 최솟값 = 1
        {
            if (max < index[i])
            {
                max = index[i];     //몇개 점령했는지
                mode = i;           //누구인지 / 1000
            }
        }
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (mode == p.GetComponent<Player_Control>().PV.ViewID / 1000)  //가장 많이 점령한 사람
            {
                Occupier = p;
                Occupier.GetComponent<Player_Control>().star++;
                most_jumlyung = max;
            }
        }

        if (mode == 0)      //점령자가 아무도 없으면
        {
            Occupier = null;
        }
    }

    void Plunder()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (most_yaktal <= p.GetComponent<Player_Control>().yaktal)   //최대 약탈 수 갱신
            {
                if (most_yaktal == p.GetComponent<Player_Control>().yaktal)  //약탈수가 같으면
                {
                    Plunderer = null;   //약탈자 없음
                }
                else
                {
                    most_yaktal = p.GetComponent<Player_Control>().yaktal;
                    Plunderer = p;  //약탈자
                    Plunderer.GetComponent<Player_Control>().star++;
                }
            }

        }
    }

    void Arena()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            for (int i = 0; i < 3; i++)
            {
                if(i == p.GetComponent<Player_Control>().PV.ViewID / 1000 - 1)       //1004, 2004, 3004 > 0, 1, 2
                {
                    p.GetComponent<Player_Control>().star += arena[i];
                }
            }
            
        }
    }

    void Kill()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int mostKill = 0;
        foreach (GameObject p in players)
        {
            if(mostKill < p.GetComponent<Player_Control>().kill_point)
            {
                mostKill = p.GetComponent<Player_Control>().kill_point;
                Killer = p;
            }
        }
    }

    void Level()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int topLevel = 0;
        foreach (GameObject p in players)
        {
            if (topLevel < p.GetComponent<Player_Control>().level)
            {
                topLevel = p.GetComponent<Player_Control>().level;
                Leveler = p;
            }
        }
    }

    void BossKill()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            /*
            if(p.GetComponent<Player_Control>().bosskill == true)
            {
                BossKiller = p;
            }
            */
        }
    }


    void Final_Reward()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int mostStar = 0;
        
        foreach (GameObject p in players)
        {
            if(mostStar < p.GetComponent<Player_Control>().star)
            {
                mostStar = p.GetComponent<Player_Control>().star;
                Final_Winner = p;
            }
        }


    }
}
