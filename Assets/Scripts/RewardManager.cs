using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class RewardManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static RewardManager Instance;

    public PhotonView PV;

    public GameObject RankingPanel;
    public GameObject RankingCamera;
    public SceneFader sceneFader;
    public Transform reward_position;
    public DialogueTrigger DT;

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

    public Text[] NameText;
    public Text[] KDText;
    public Text[] YaktalText;
    public Text[] MonsterKillText;
    public Transform[] StarList;
    public Transform[] OccupiedList;  //child로 검사

    public bool End;

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
        End = true;
        //StartCoroutine(Test());
    }

    IEnumerator Test()  //페이드아웃 테스트
    {
        yield return new WaitForSeconds(60f);
        Reward();
    }

    public void ToThrone()   //의자있는화면으로
    {
        RankDisplay();
        UIClose();  //UI끄기
        Debug.Log("@@@@@@@@@@@ToChair@@@@@@@");
        End = true;
        First_Player.transform.position = reward_position.position;
        First_Player.transform.rotation = reward_position.rotation;

    }

    public void FadeInTest()
    {
        StartCoroutine(FadeInTest_());
    }

    IEnumerator FadeInTest_() //페이드인 테스트
    {

        Debug.Log("@@@@@@@@@@@Test3@@@@@@@");
        yield return new WaitForSeconds(2f);
        RankingCamera.SetActive(true);      //랭킹카메라 켜기

        sceneFader.FadeIn();
    }

    public void RewardPlace()   //Fadein 마지막
    {
        Debug.Log("@@@@@@@@@@@RewardPlace@@@@@@@");

        StartCoroutine(RankingDisplay());
    }

    IEnumerator RankingDisplay()
    {
        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(1.0f);
        RankingPanel.SetActive(true);

        StartCoroutine(Star(0, First_Player.GetComponent<Player_Control>().star));
        StartCoroutine(Star(1, Second_Player.GetComponent<Player_Control>().star));
        StartCoroutine(Star(2, Third_Player.GetComponent<Player_Control>().star));
    }

    public void Reward()    //마지막 아레나 끝나면 호출
    {

        Occupy();   //점령
        Plunder();  //약탈
        //Arena();
        Kill();
        Level();
        BossKill();
        Final_Reward();

        sceneFader.FadeOut();   //페이드 아웃
    }


    void Occupy()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int[] Oc = { 0, 0, 0};  //각자 몇개씩 점령했는지
        foreach (GameObject p in players)
        {
            Player_Control player_data_ = p.GetComponent<Player_Control>();
            int index = player_data_.PV.ViewID / 1000 - 1;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (player_data_.QD.questClearCheck[j] == true && player_data_.QD.questIsActive[j] == false)
                    {
                        Oc[index]++;
                        OccupiedList[i].GetChild(j).gameObject.SetActive(true);
                        Debug.Log(i + "번째 리스트" + j + "번째 요소 활성화");
                    }
                }
            }

        }

        int max = 0;
        int ocId = 0;
        for (int i = 0; i < 3; i++)  //1부터 > 플레이어 / 1000의 최솟값 = 1
        {
            if (max < Oc[i])
            {
                max = Oc[i];     //몇개 점령했는지
                ocId = i;
            }
        }

        foreach (GameObject p in players)
        {
            Player_Control player_data_ = p.GetComponent<Player_Control>();


            if (ocId == player_data_.PV.ViewID / 1000 - 1)
            {
                Occupier = p;
            }

        }


        if (Occupier != null)
            Occupier.GetComponent<Player_Control>().star++;

    }


    void Plunder()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (most_yaktal < p.GetComponent<Player_Control>().yaktal)   //최대 약탈 수 갱신
            {/*
                if (most_yaktal == p.GetComponent<Player_Control>().yaktal)  //약탈수가 같으면
                {
                    Plunderer = null;   //약탈자 없음
                }
                else
                {*/
                    most_yaktal = p.GetComponent<Player_Control>().yaktal;
                    Plunderer = p;  //약탈자

                //}
            }

        }
        
        if (Plunderer != null)
            Plunderer.GetComponent<Player_Control>().star++;
    }
    /*
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
    */
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
        if (Killer != null)
            Killer.GetComponent<Player_Control>().star++;
    }

    void Level()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int topLevel = 1;
        foreach (GameObject p in players)
        {
            if (topLevel < p.GetComponent<Player_Control>().level)
            {
                topLevel = p.GetComponent<Player_Control>().level;
                Leveler = p;

            }
        }
        if (Leveler != null)
            Leveler.GetComponent<Player_Control>().star++;
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

    void Final_Reward()     //순위 계산
    {
        
        RankingCamera.SetActive(true);      //랭킹카메라 켜기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            int index = p.GetComponent<Player_Control>().PV.ViewID / 1000 - 1;
            Debug.Log(index);
            RankPlayers[index] = p;
        }


        for (int i = 1; i < 2; i++) //1
        {
            for (int j = 0; j < 2; j++) //0, 1
            {
                if (RankPlayers[j].GetComponent<Player_Control>().star >= RankPlayers[j + 1].GetComponent<Player_Control>().star)   //앞이 크거나 같으면
                {
                    if (RankPlayers[j].GetComponent<Player_Control>().star == RankPlayers[j + 1].GetComponent<Player_Control>().star)
                    {
                        if(RankPlayers[j].GetComponent<Player_Control>().score < RankPlayers[j + 1].GetComponent<Player_Control>().score)
                        {
                            GameObject temp = RankPlayers[j];
                            RankPlayers[j] = RankPlayers[j + 1];
                            RankPlayers[j + 1] = temp;
                        }
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
                if (RankPlayers[i].GetComponent<Player_Control>().star == RankPlayers[i + 1].GetComponent<Player_Control>().star)
                {
                    if (RankPlayers[i].GetComponent<Player_Control>().score < RankPlayers[i + 1].GetComponent<Player_Control>().score)
                    {
                        GameObject temp = RankPlayers[i];
                        RankPlayers[i] = RankPlayers[i + 1];
                        RankPlayers[i + 1] = temp;
                    }

                }
            }
            else        //앞이 작으면 바꾸기
            {
                GameObject temp = RankPlayers[i];
                RankPlayers[i] = RankPlayers[i + 1];
                RankPlayers[i + 1] = temp;
            }

        }
        /*
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                p.GetComponent<Player_Control>().characterCamera.gameObject.SetActive(false);
                //p.GetComponent<Player_Control>().minimapCamera.gameObject.SetActive(false);
                p.GetComponent<Player_Control>().WeaponPosition_L.SetActive(false);
                p.GetComponent<Player_Control>().WeaponPosition_R.SetActive(false);

                p.GetComponent<Player_Control>().enabled = false;
            }

        }

    */
        First_Player = RankPlayers[0];
        Second_Player = RankPlayers[1];
        Third_Player = RankPlayers[2];


    }

    void UIClose()  //각 플레이어 인터페이스 끄기
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                p.GetComponent<Player_Control>().enabled = false;
                p.transform.parent.GetChild(1).gameObject.SetActive(false); //인터페이스캔버스 끄기
                Destroy(p.GetComponent<Player_Control>().characterCamera.gameObject); 
                p.GetComponent<Player_Control>().characterCamera.GetComponent<AudioListener>().enabled = false;
                //p.GetComponent<Player_Control>().minimapCamera.gameObject.SetActive(false);
                //p.GetComponent<Player_Control>().WeaponPosition_L.SetActive(false);
                //p.GetComponent<Player_Control>().WeaponPosition_R.SetActive(false);

                //p.GetComponent<Player_Control>().enabled = false;
            }
        }
    }

    public void RankDisplay()   //랭킹 내용 작성
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            Player_Control player_data_ = p.GetComponent<Player_Control>();

            if(p == First_Player)    //1등
            {
                NameText[0].text = player_data_.username;
                KDText[0].text = player_data_.kill_point + " / " + player_data_.death_point;
                YaktalText[0].text = player_data_.yaktal.ToString();
                MonsterKillText[0].text = player_data_.monster_killpoint.ToString();
                /*
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
                }*/
            }
            else if(p == Second_Player)     //2등
            {
                NameText[1].text = player_data_.username;
                KDText[1].text = player_data_.kill_point + " / " + player_data_.death_point;
                YaktalText[1].text = player_data_.yaktal.ToString();
                MonsterKillText[1].text = player_data_.monster_killpoint.ToString();
                /*
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
                }*/
            }
            else        //3등
            {
                NameText[2].text = player_data_.username;
                KDText[2].text = player_data_.kill_point + " / " + player_data_.death_point;
                YaktalText[2].text = player_data_.yaktal.ToString();
                MonsterKillText[2].text = player_data_.monster_killpoint.ToString();
                /*
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
                }*/
            }

        }

    }

    IEnumerator Star(int index, int num)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < num; i++)
        {
            yield return new WaitForSeconds(0.1f);
            StarList[index].transform.GetChild(i).gameObject.SetActive(true);
        }

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
        //PhotonNetwork.Disconnect();
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        SceneManager.LoadScene(0);
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LoadLevel(0);
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(End);
        }
        else
        {
            End = (bool)stream.ReceiveNext();
        }
    }
}
