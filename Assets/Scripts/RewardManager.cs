using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RewardManager : MonoBehaviourPunCallbacks
{
    public static RewardManager Instance;

    public SceneFader sceneFader;

    public int most_jumlyung;   //�ִ� ���� ��
    public int most_yaktal;     //�ִ� ��Ż ��


    public GameObject Occupier;    //���� ���� ������ ��
    public GameObject Plunderer;    //���� ���� ��Ż�� ��
    public GameObject Killer;    //���� ���� ų�� ��
    public GameObject Leveler;    //���� ���� ų�� ��
    public GameObject BossKiller;    //���� ���� ų�� ��

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

    IEnumerator Test()  //���̵�ƿ� �׽�Ʈ
    {
        yield return new WaitForSeconds(5f);
        Reward();
    }

    public void Tochair()   //�����ִ�ȭ������
    {
        StartCoroutine(Test3());
    }

    IEnumerator Test3() //���̵��� �׽�Ʈ
    {
        yield return new WaitForSeconds(2f);
        sceneFader.FadeIn();
    }

    public void Reward()
    {
        sceneFader.FadeOut();


        Occupy();   //����
        Plunder();  //��Ż
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
            sectionOwner[i] = QM.GetComponent<QuestManager>().SectionOwner[i] / 1000;   //�� ������ ������ viewID�� 1000���� ������
        }

        int[] index = { 0, 0, 0, 0 };   //3�� 3����
        for (int i = 0; i < index.Length; i++)
        {
            index[sectionOwner[i]]++;
        }

        int max = 0;
        int mode = 0;
        for (int i = 1; i < index.Length; i++)  //1���� > �÷��̾� / 1000�� �ּڰ� = 1
        {
            if (max < index[i])
            {
                max = index[i];     //� �����ߴ���
                mode = i;           //�������� / 1000
            }
        }
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (mode == p.GetComponent<Player_Control>().PV.ViewID / 1000)  //���� ���� ������ ���
            {
                Occupier = p;
                Occupier.GetComponent<Player_Control>().star++;
                most_jumlyung = max;
            }
        }

        if (mode == 0)      //�����ڰ� �ƹ��� ������
        {
            Occupier = null;
        }
    }

    void Plunder()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (most_yaktal <= p.GetComponent<Player_Control>().yaktal)   //�ִ� ��Ż �� ����
            {
                if (most_yaktal == p.GetComponent<Player_Control>().yaktal)  //��Ż���� ������
                {
                    Plunderer = null;   //��Ż�� ����
                }
                else
                {
                    most_yaktal = p.GetComponent<Player_Control>().yaktal;
                    Plunderer = p;  //��Ż��
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
