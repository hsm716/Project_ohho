using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RewardManager : MonoBehaviourPunCallbacks
{
    public static RewardManager Instance;

    public GameObject RankingPanel;
    public GameObject RankCamera;

    public SceneFader sceneFader;
    public Transform reward_position;

    public GameObject[] RankDetail;
    public GameObject[] FoldoutButton;
    public GameObject[] DetailPanel;

    public int most_jumlyung;   //�ִ� ���� ��
    public int most_yaktal;     //�ִ� ��Ż ��


    public GameObject Occupier;    //���� ���� ������ ��
    public GameObject Plunderer;    //���� ���� ��Ż�� ��
    public GameObject Killer;    //���� ���� ų�� ��
    public GameObject Leveler;    //���� ���� ų�� ��
    public GameObject BossKiller;    //���� ���� ų�� ��

    public GameObject First_Player;     //1��
    public GameObject Second_Player;    //2��
    public GameObject Third_Player;     //3��

    public GameObject[] RankPlayers = { null, null, null };
    public int[] arena = { 0, 0, 0 };
    public int[] stars = { 0, 0, 0 };
    public int[] score = { 0, 0, 0 };

    public Text[] NameText;
    public Text[] KDText;
    public Transform[] StarList;
    public Transform[] OccupiedList;  //child�� �˻�

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

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                First_Player = p;
                Second_Player = p;
                Third_Player = p;
            }

        }




        StartCoroutine(Test());
    }

    IEnumerator Test()  //���̵�ƿ� �׽�Ʈ
    {
        yield return new WaitForSeconds(30f);
        Reward();
    }

    public void Tochair()   //�����ִ�ȭ������
    {
        StartCoroutine(Test3());
    }

    IEnumerator Test3() //���̵��� �׽�Ʈ
    {
        yield return new WaitForSeconds(2f);
        RankCamera.SetActive(true);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if(p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                p.GetComponent<Player_Control>().characterCamera.gameObject.SetActive(false);
                //p.GetComponent<Player_Control>().minimapCamera.gameObject.SetActive(false);
                p.GetComponent<Player_Control>().WeaponPosition_L.SetActive(false);
                p.GetComponent<Player_Control>().WeaponPosition_R.SetActive(false);

                p.GetComponent<Player_Control>().enabled = false;
            }

        }

        First_Player.transform.position = reward_position.position;
        First_Player.transform.rotation = reward_position.rotation;
        //�ɴ� �ִϸ��̼�
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
        UIClose();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //int mostStar = 0;
        
        foreach (GameObject p in players)
        {
            Player_Control player_data_ = p.GetComponent<Player_Control>();
            int index = player_data_.PV.ViewID / 1000 - 1;
            RankPlayers[index] = p; //���� ������� ���� �� ��Ÿ�� ���� ����� ������
        }

        for (int i = 1; i < 2; i++) //1
        {
            for (int j = 0; j < 2; j++) //0, 1
            {
                if (RankPlayers[j].GetComponent<Player_Control>().star >= RankPlayers[j + 1].GetComponent<Player_Control>().star)   //���� ���� ���ų� ������
                {
                    if (RankPlayers[j].GetComponent<Player_Control>().star == RankPlayers[j + 1].GetComponent<Player_Control>().star)   //���� ������
                    {
                        if (RankPlayers[j].GetComponent<Player_Control>().score < RankPlayers[j + 1].GetComponent<Player_Control>().score)   //���� ���ھ ������
                        {
                            GameObject temp = RankPlayers[j];
                            RankPlayers[j] = RankPlayers[j + 1];
                            RankPlayers[j + 1] = temp;
                        }
                    }
                }
                else    //�պ��� ���� ������ �ٲٱ�
                {
                    GameObject temp = RankPlayers[j];
                    RankPlayers[j] = RankPlayers[j + 1];
                    RankPlayers[j + 1] = temp;
                }

            }
            // i = 1
            if (RankPlayers[i].GetComponent<Player_Control>().star >= RankPlayers[i + 1].GetComponent<Player_Control>().star)   //���� ũ�ų� ������
            {
                if (RankPlayers[i].GetComponent<Player_Control>().star == RankPlayers[i + 1].GetComponent<Player_Control>().star)     //���� ������ 
                {
                    if (RankPlayers[i].GetComponent<Player_Control>().score < RankPlayers[i + 1].GetComponent<Player_Control>().score)   //���� ���ھ ������
                    {
                        GameObject temp = RankPlayers[i];
                        RankPlayers[i] = RankPlayers[i + 1];
                        RankPlayers[i + 1] = temp;
                    }
                }
            }
            else        //���� ������ �ٲٱ�
            {
                GameObject temp = RankPlayers[i];
                RankPlayers[i] = RankPlayers[i + 1];
                RankPlayers[i + 1] = temp;
            }
        }


        First_Player = RankPlayers[0];
        Second_Player = RankPlayers[1];
        Third_Player = RankPlayers[2];

        RankDisplay();
    }
     
    void UIClose()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            p.transform.parent.GetChild(1).gameObject.SetActive(false); //������ġ��ĵ���� ����
        }
    }

    public void RankDisplay()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        Player_Control first_player_date = First_Player.GetComponent<Player_Control>();
        Player_Control second_player_date = Second_Player.GetComponent<Player_Control>();
        Player_Control third_player_date = Third_Player.GetComponent<Player_Control>();



        NameText[0].text = first_player_date.username;
        KDText[0].text = first_player_date.kill_point + " / " + first_player_date.death_point;

        for (int j = 0; j < 5; j++) //������
        {
            if (first_player_date.QD.questClearCheck[j] == true && first_player_date.QD.questIsActive[j] == false)
            {
                OccupiedList[0].transform.GetChild(j).gameObject.SetActive(true);
            }
            else
            {
                OccupiedList[0].transform.GetChild(j).gameObject.SetActive(false);
            }
        }


        NameText[1].text = second_player_date.username;
        KDText[1].text = second_player_date.kill_point + " / " + second_player_date.death_point;

        for (int j = 0; j < 5; j++) //������
        {
            if (second_player_date.QD.questClearCheck[j] == true && second_player_date.QD.questIsActive[j] == false)
            {
                OccupiedList[1].transform.GetChild(j).gameObject.SetActive(true);

            }
            else
            {
                OccupiedList[1].transform.GetChild(j).gameObject.SetActive(false);
            }
        }

        NameText[2].text = third_player_date.username;
        KDText[2].text = third_player_date.kill_point + " / " + third_player_date.death_point;

        for (int j = 0; j < 5; j++) //������
        {
            if (third_player_date.QD.questClearCheck[j] == true && third_player_date.QD.questIsActive[j] == false)
            {
                OccupiedList[2].transform.GetChild(j).gameObject.SetActive(true);
            }
            else
            {
                OccupiedList[2].transform.GetChild(j).gameObject.SetActive(false);
            }
        }

    }

    IEnumerator Star(int index, int num)
    {
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < num; i++)
        {
            yield return new WaitForSeconds(0.1f);
            StarList[index].transform.GetChild(i).gameObject.SetActive(true);
        }

    }

    public void ShowPanel()
    {
        RankingPanel.SetActive(true);

        StartCoroutine(Star(0, First_Player.GetComponent<Player_Control>().star));
        StartCoroutine(Star(1, Second_Player.GetComponent<Player_Control>().star));
        StartCoroutine(Star(2, Third_Player.GetComponent<Player_Control>().star));

    }

    public void Detail_FoldOut(int num) //��ġ��
    {
        RankDetail[num].GetComponent<Animator>().SetBool("Fold", true);
        FoldoutButton[num].SetActive(false);
        StartCoroutine(Detail_delay(num));
    }
    public void Detail_FoldIn(int num) //����
    {
        RankDetail[num].GetComponent<Animator>().SetBool("Fold", false);
        DetailPanel[num].SetActive(false);
        FoldoutButton[num].SetActive(true);
    }

    IEnumerator Detail_delay(int num)
    {
        yield return new WaitForSeconds(0.5f);
        DetailPanel[num].SetActive(true);
    }

    public void GotoMenu()
    {
        Destroy(RoomManager.Instance.gameObject);
        Destroy(QuestManager.Instance.gameObject);
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
        Destroy(gameObject);
    }
}
