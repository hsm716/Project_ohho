using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RewardManager : MonoBehaviourPunCallbacks
{
    public static RewardManager Instance;
    public SceneFader sceneFader;
    public Transform reward_position;

    public GameObject[] RankDetail;
    public GameObject[] FoldoutButton;
    public GameObject[] DetailPanel;

    public int most_jumlyung;   //최대 점령 수
    public int most_yaktal;     //최대 약탈 수


    public GameObject Occupier;    //가장 많이 점령한 자
    public GameObject Plunderer;    //가장 많이 약탈한 자
    public GameObject Killer;    //가장 많이 킬한 자
    public GameObject Leveler;    //가장 많이 킬한 자
    public GameObject BossKiller;    //가장 많이 킬한 자

    public GameObject First_Player;     //1등
    public GameObject Second_Player;    //2등
    public GameObject Third_Player;     //3등

    public GameObject[] RankPlayers;
    public int[] arena = { 0, 0, 0 };
    public int[] score = { 0, 0, 0 };

    public Text[] NameText;
    public Text[] KDText;
    public Transform[] StarList;
    public Transform[] OccupiedList;  //child로 검사

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
        UIClose();

        int[] stars = { 0, 0, 0 };
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int mostStar = 0;
        
        foreach (GameObject p in players)
        {
            /*
            if(mostStar < p.GetComponent<Player_Control>().star)
            {
                mostStar = p.GetComponent<Player_Control>().star;
                First_Player = p;
            }


            */

            Player_Control player_data_ = p.GetComponent<Player_Control>();
            int index = player_data_.PV.ViewID / 1000 - 1;
            RankPlayers[index] = p;

            stars[index] = player_data_.star;
            //score[index] += player_data_.kill_point * 10 + player_data_.monster_kill_point * 2;

            for (int i = 1; i < 2; i++) //1
            {
                for (int j = 0; j < 2; j++) //0, 1
                {
                    if (RankPlayers[j].GetComponent<Player_Control>().star >= RankPlayers[j + 1].GetComponent<Player_Control>().star)   //앞이 크거나 같으면
                    {
                        if (RankPlayers[j].GetComponent<Player_Control>().score == RankPlayers[j + 1].GetComponent<Player_Control>().score)
                        {
                            GameObject temp = RankPlayers[j];
                            RankPlayers[j] = RankPlayers[j + 1];
                            RankPlayers[j + 1] = temp;
                        }
                    }
                    else    //앞이 작으면 바꾸기
                    {
                        GameObject temp = RankPlayers[j];
                        RankPlayers[j] = RankPlayers[j + 1];
                        RankPlayers[j + 1] = temp;
                    }

                }

                if (RankPlayers[i].GetComponent<Player_Control>().star >= RankPlayers[i + 1].GetComponent<Player_Control>().star)   //앞이 크거나 같으면
                {
                    if (RankPlayers[i].GetComponent<Player_Control>().score == RankPlayers[i + 1].GetComponent<Player_Control>().score)
                    {
                        GameObject temp = RankPlayers[i];
                        RankPlayers[i] = RankPlayers[i + 1];
                        RankPlayers[i + 1] = temp;
                    }
                }
                else        //앞이 작으면 바꾸기
                {
                    GameObject temp = RankPlayers[i];
                    RankPlayers[i] = RankPlayers[i + 1];
                    RankPlayers[i + 1] = temp;
                }

            }

            
            
            
        }

        foreach (GameObject player in RankPlayers)
        {



        }





        RankDisplay();

        First_Player.transform.position = reward_position.position;
        First_Player.transform.rotation = reward_position.rotation;
    }

    void UIClose()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            p.transform.parent.GetChild(1).gameObject.SetActive(false); //인터페치스캔버스 끄기
        }
    }

    public void RankDisplay()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            Player_Control player_data_ = p.GetComponent<Player_Control>();

            if(p == First_Player)    //1등
            {
                NameText[0].text = player_data_.username;
                KDText[0].text = player_data_.kill_point + " / " + player_data_.death_point;

                for (int j = 0; j < 5; j++) //점령지
                {
                    if (player_data_.QD.questClearCheck[j] == true && player_data_.QD.questIsActive[j] == false)
                    {
                        OccupiedList[0].transform.GetChild(j).gameObject.SetActive(true);
                    }
                    else
                    {
                        OccupiedList[0].transform.GetChild(j).gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < player_data_.star; i++) //별
                {
                    StartCoroutine(Star(0, i)); //딜레이
                }
            }
            else if(p == Second_Player)     //2등
            {
                NameText[1].text = player_data_.username;
                KDText[1].text = player_data_.kill_point + " / " + player_data_.death_point;

                for (int j = 0; j < 5; j++) //점령지
                {
                    if (player_data_.QD.questClearCheck[j] == true && player_data_.QD.questIsActive[j] == false)
                    {
                        OccupiedList[1].transform.GetChild(j).gameObject.SetActive(true);

                    }
                    else
                    {
                        OccupiedList[1].transform.GetChild(j).gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < player_data_.star; i++) //별
                {
                    StartCoroutine(Star(2, i)); //딜레이
                }
            }
            else        //3등
            {
                NameText[2].text = player_data_.username;
                KDText[2].text = player_data_.kill_point + " / " + player_data_.death_point;

                for (int j = 0; j < 5; j++) //점령지
                {
                    if (player_data_.QD.questClearCheck[j] == true && player_data_.QD.questIsActive[j] == false)
                    {
                        OccupiedList[2].transform.GetChild(j).gameObject.SetActive(true);
                    }
                    else
                    {
                        OccupiedList[2].transform.GetChild(j).gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < player_data_.star; i++) //별
                {
                    StartCoroutine(Star(2, i)); //딜레이
                }
            }


        }
    }

    IEnumerator Star(int index, int num)
    {
        yield return new WaitForSeconds(0.1f);
        OccupiedList[index].transform.GetChild(num).gameObject.SetActive(true);
    }



    public void Detail_FoldOut(int num) //펼치기
    {
        RankDetail[num].GetComponent<Animator>().SetBool("Fold", true);
        FoldoutButton[num].SetActive(false);
        StartCoroutine(Detail_delay(num));
    }
    public void Detail_FoldIn(int num) //접기
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
