using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Interface : MonoBehaviour
{

    public Player_Arena PA;
    //수정 테스트
    public Player_Control player_data;
    public Slider expBar;
    public TextMeshProUGUI expPercent;

    public TextMeshProUGUI HP_UI;

    public Slider HPBAR;

    public GameObject LevelUpPanel;
    public TextMeshProUGUI Next_Level;

    public Image[] select_img;
    //public Text[] select_name;
    public TextMeshProUGUI[] select_name;

    public Sprite[] select_ability_icons;
    string[] select_ability_names = { "HP", "SPD", "ATK", "DEF", "STAMINA", "CRITICAL" };


    public Image[] Inventory_item_img;
    public TextMeshProUGUI[] Inventory_item_txt;
    public int[] Inventory_item_num;
    public string[] Inventory_item_name;

    public Dictionary<string,int> item_Material;

    public TextMeshProUGUI time_txt;

    public float time;
    public Slider timeSlider;
    public GameObject MC;

    public GameManager gm;
    //리얼 수정좀 잘좀 해주세요;; ㅅㅂ
    // Update is called once per frame
    // Im so sad, wy
    public GameObject GameBoard_Tab;


    public bool isArena = false;
    public bool isActive_Input;

    public bool isArena_in = false;

    public RawImage minimap_Renderer;



    bool atk_possible;
    bool def_possible;
    bool spd_possible;
    bool critical_possible;
    bool hp_possible;
    bool stamina_possible;

    int atk_count = 0;
    int def_count = 0;
    int spd_count = 0;
    int critical_count = 0;
    int hp_count = 0;
    int stamina_count = 0;

    public Image RedBuff_image;
    public Image BlueBuff_image;
    public Image GreenBuff_image;
    public GameObject Respawn_Arena;

    
    public TextMeshProUGUI curDefense_degree_txt;
    public TextMeshProUGUI curAttack_degree_txt;
    public TextMeshProUGUI curSpeed_degree_txt;
    public TextMeshProUGUI curCritical_degree_txt;
    public TextMeshProUGUI curHP_degree_txt;
    public TextMeshProUGUI curStamina_degree_txt;

    public TextMeshProUGUI player_level_txt;
    private void Awake()
    {
        isActive_Input = true;
        gm = GameObject.Find("GameTime").GetComponent<GameManager>();
        MC = GameObject.Find("MainCanvas");
        
        
        
        time = gm.arena_time;
        string name;
        item_Material = new Dictionary<string, int>();

        name = "twig";
        item_Material.Add(name, 0);
        name = "forest_spirit";
        item_Material.Add(name, 0);


        atk_possible = false ;
        def_possible = false;
        spd_possible = false;
        critical_possible = false;
        hp_possible = false;
        stamina_possible = false;

        atk_count = 0;
        def_count = 0;
        spd_count = 0;
        critical_count = 0;
        hp_count = 0;
        stamina_count = 0;

    }
    private void Start()
    {
        Respawn_Arena = GameObject.Find("Respawn_Arena");
    }
    void Update()
    {
        if(atk_count>=3)
            atk_possible = true;
        if (def_count >= 3)
            def_possible = true;
        if (spd_count >= 3)
            spd_possible = true;
        if (critical_count >= 3)
            critical_possible = true;
        if (hp_count >= 3)
            hp_possible = true;
        if (stamina_count >= 3)
            stamina_possible = true;

        time = gm.arena_time;
        time_txt.text = string.Format("{0:00}",(int)(time/60)) + " : " + string.Format("{0:00}", (int)(time % 60));
        timeSlider.value = time / 300f;
        curDefense_degree_txt.text = def_count +" / 3";
        curAttack_degree_txt.text = atk_count +" / 3";
        curSpeed_degree_txt.text = spd_count +" / 3";
        curHP_degree_txt.text = hp_count +" / 3";
        curCritical_degree_txt.text = critical_count +" / 3";
        curStamina_degree_txt.text = stamina_count +" / 3";

        player_level_txt.text = ""+player_data.level;

        if (time <= 0f&&player_data.isArena==false)
        {
            ArenaIn();
        }
        if(time <= 0f && player_data.isArena==true)
        {
            ArenaOut();
        }
        if(player_data.isArena == true)
        {
            CheckFinished_Arena();
        }
        //CheckFinished_Arena();
        HP_UI.text = (int)player_data.curHP +" / " + player_data.maxHP;
        
        HPBAR.value = (player_data.curHP / player_data.maxHP);
        expBar.value = (player_data.curEXP / player_data.maxEXP) * 100;
        Next_Level.text = "" + player_data.level;
        expPercent.text = string.Format("{0:0.00}", ((player_data.curEXP / player_data.maxEXP) * 100)) + "%";

        RedBuff_image.fillAmount = player_data.redBuff_time / 100f;
        BlueBuff_image.fillAmount = player_data.blueBuff_time / 100f;
        GreenBuff_image.fillAmount = player_data.greenBuff_time / 100f;

    }
    void ArenaIn()
    {
        player_data.horizontalMove = 0f;
        player_data.verticalMove = 0f;
        player_data.arenaWin = true;
        GameManager.Instance.isActive = false;
        isActive_Input = false;
        GameObject arenaCanvas = MC.transform.GetChild(7).gameObject;
        arenaCanvas.SetActive(true);
        arenaCanvas.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ArenaOut()
    {
        isActive_Input = true;
        gm.arena_time = 30f;
        player_data.isArena = false;
        isArena_in = false;

/*        if (player_data.arenaWin)
        {
            player_data.arenaRank = GameManager.Instance.arenaRank-1;
        }*/
       

        if (player_data.PV.IsMine)
        {
            GameObject[] soldiers = GameObject.FindGameObjectsWithTag("Soldier");
            foreach (var s in soldiers)
            {
                Destroy(s);
            }
        }

        player_data.Invoke("Respawn",3f);

/*        player_data.rgbd.isKinematic = true;
        player_data.transform.position = player_data.Respawn_Center.transform.GetChild((int)(player_data.PV.ViewID / 1000)-1).transform.position;
        player_data.rgbd.isKinematic = false;*/


        GameObject arenaCanvas = MC.transform.GetChild(7).gameObject;
        arenaCanvas.transform.GetChild(2).gameObject.SetActive(true);

        if (player_data.arenaWin)
        {
            player_data.arenaRank=1;
            if(GameManager.Instance.areanaCount == 3)   //마지막 
            {
                RewardManager.Instance.arena[player_data.PV.ViewID / 1000 - 1] += 2;
            }
            else
            {
                RewardManager.Instance.arena[player_data.PV.ViewID / 1000 - 1]++;
            }
        }
        if (GameManager.Instance.areanaCount == 3)
            RewardManager.Instance.Reward();    //정산
        player_data.PV.RPC("initRank", RpcTarget.All);

        //PA.ranking_img.sprite = PA.ranking_123_sp[PA.rank-1];

    }

    void CheckFinished_Arena()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int count = 0;
        foreach (var player in players)
        {
            Player_Control pc = player.GetComponent<Player_Control>();
            if (pc.isDeath)
            {
                count += 1;
            }
        }
        if (count >= GameManager.Instance.ReadyCountMax - 1)
        {
            if (player_data.arenaWin)
            {
                
            }
            ArenaOut();
        }
    }

    public void Select(int index)
    {
        //qwe
        switch (select_name[index].text)
        {
            case "HP":
                HpUp();
                hp_count += 1;
                break;
            case "SPD":
                SpeedUp();
                spd_count += 1;
                break;
            case "ATK":
                PowerUp();
                atk_count += 1;
                break;
            case "DEF":
                def_count += 1;
                break;
            case "CRITICAL":
                critical_count += 1;
                break;
            case "STAMINA":
                stamina_count += 1;
                break;

        }
        LevelUpPanel.SetActive(false);

    }
    public void HpUp()
    {
        player_data.maxHP += 1000;
        player_data.Hp_Bar.GetHpBoost();
    }
    public void SpeedUp()
    {
        player_data.walkSpeed += 1;
        player_data.curSpeed = player_data.walkSpeed;
        player_data.animator.SetFloat("RunningAmount", player_data.animator.GetFloat("RunningAmount") + 0.2f);
    }
    public void PowerUp()
    {
        player_data.atk *= 1.25f;
    }

    IEnumerator Shuffle()
    {
        //qq
        yield return new WaitForSeconds(1f);

        bool[] selected_state = { hp_possible, spd_possible, atk_possible, def_possible, critical_possible,stamina_possible };

        int count = 0;
        while (count < 3)
        {
            int rand_idx = Random.Range(0, 6);
            if (selected_state[rand_idx] == false)
            {
                selected_state[rand_idx] = true;
                select_img[count].sprite = select_ability_icons[rand_idx];
                select_name[count].text = select_ability_names[rand_idx];
                count++;
            }
            else
            {
                continue;
            }
        }
        LevelUpPanel.SetActive(true);
    }
    
}
