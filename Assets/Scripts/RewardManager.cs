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

    public GameObject[] RankPlayers;

    public Text[] NameText;
    public Text[] KDText;
    public Text[] YaktalText;
    public Text[] MonsterKillText;
    public Transform[] StarList;
    public Transform[] OccupiedList;  //child�� �˻�

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

    IEnumerator Test()  //���̵�ƿ� �׽�Ʈ
    {
        yield return new WaitForSeconds(60f);
        Reward();
    }

    public void ToThrone()   //�����ִ�ȭ������
    {
        RankDisplay();
        UIClose();  //UI����
        Debug.Log("@@@@@@@@@@@ToChair@@@@@@@");
        End = true;
        First_Player.transform.position = reward_position.position;
        First_Player.transform.rotation = reward_position.rotation;

    }

    public void FadeInTest()
    {
        StartCoroutine(FadeInTest_());
    }

    IEnumerator FadeInTest_() //���̵��� �׽�Ʈ
    {

        Debug.Log("@@@@@@@@@@@Test3@@@@@@@");
        yield return new WaitForSeconds(2f);
        RankingCamera.SetActive(true);      //��ŷī�޶� �ѱ�

        sceneFader.FadeIn();
    }

    public void RewardPlace()   //Fadein ������
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

    public void Reward()    //������ �Ʒ��� ������ ȣ��
    {

        Occupy();   //����
        Plunder();  //��Ż
        //Arena();
        Kill();
        Level();
        BossKill();
        Final_Reward();

        sceneFader.FadeOut();   //���̵� �ƿ�
    }


    void Occupy()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int[] Oc = { 0, 0, 0};  //���� ��� �����ߴ���
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
                        Debug.Log(i + "��° ����Ʈ" + j + "��° ��� Ȱ��ȭ");
                    }
                }
            }

        }

        int max = 0;
        int ocId = 0;
        for (int i = 0; i < 3; i++)  //1���� > �÷��̾� / 1000�� �ּڰ� = 1
        {
            if (max < Oc[i])
            {
                max = Oc[i];     //� �����ߴ���
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
            if (most_yaktal < p.GetComponent<Player_Control>().yaktal)   //�ִ� ��Ż �� ����
            {/*
                if (most_yaktal == p.GetComponent<Player_Control>().yaktal)  //��Ż���� ������
                {
                    Plunderer = null;   //��Ż�� ����
                }
                else
                {*/
                    most_yaktal = p.GetComponent<Player_Control>().yaktal;
                    Plunderer = p;  //��Ż��

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

    void Final_Reward()     //���� ���
    {
        
        RankingCamera.SetActive(true);      //��ŷī�޶� �ѱ�
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
                if (RankPlayers[j].GetComponent<Player_Control>().star >= RankPlayers[j + 1].GetComponent<Player_Control>().star)   //���� ũ�ų� ������
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
                else    //���� ������ �ٲٱ�
                {
                    GameObject temp = RankPlayers[j];
                    RankPlayers[j] = RankPlayers[j + 1];
                    RankPlayers[j + 1] = temp;
                }

            }

            if (RankPlayers[i].GetComponent<Player_Control>().star >= RankPlayers[i + 1].GetComponent<Player_Control>().star)   //���� ũ�ų� ������
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
            else        //���� ������ �ٲٱ�
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

    void UIClose()  //�� �÷��̾� �������̽� ����
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.GetComponent<Player_Control>().PV.Owner.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                p.GetComponent<Player_Control>().enabled = false;
                p.transform.parent.GetChild(1).gameObject.SetActive(false); //�������̽�ĵ���� ����
                Destroy(p.GetComponent<Player_Control>().characterCamera.gameObject); 
                p.GetComponent<Player_Control>().characterCamera.GetComponent<AudioListener>().enabled = false;
                //p.GetComponent<Player_Control>().minimapCamera.gameObject.SetActive(false);
                //p.GetComponent<Player_Control>().WeaponPosition_L.SetActive(false);
                //p.GetComponent<Player_Control>().WeaponPosition_R.SetActive(false);

                //p.GetComponent<Player_Control>().enabled = false;
            }
        }
    }

    public void RankDisplay()   //��ŷ ���� �ۼ�
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            Player_Control player_data_ = p.GetComponent<Player_Control>();

            if(p == First_Player)    //1��
            {
                NameText[0].text = player_data_.username;
                KDText[0].text = player_data_.kill_point + " / " + player_data_.death_point;
                YaktalText[0].text = player_data_.yaktal.ToString();
                MonsterKillText[0].text = player_data_.monster_killpoint.ToString();
                /*
                for (int j = 0; j < 5; j++) //������
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
            else if(p == Second_Player)     //2��
            {
                NameText[1].text = player_data_.username;
                KDText[1].text = player_data_.kill_point + " / " + player_data_.death_point;
                YaktalText[1].text = player_data_.yaktal.ToString();
                MonsterKillText[1].text = player_data_.monster_killpoint.ToString();
                /*
                for (int j = 0; j < 5; j++) //������
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
            else        //3��
            {
                NameText[2].text = player_data_.username;
                KDText[2].text = player_data_.kill_point + " / " + player_data_.death_point;
                YaktalText[2].text = player_data_.yaktal.ToString();
                MonsterKillText[2].text = player_data_.monster_killpoint.ToString();
                /*
                for (int j = 0; j < 5; j++) //������
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
