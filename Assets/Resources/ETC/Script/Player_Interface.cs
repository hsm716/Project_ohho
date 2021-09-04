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

    public Slider STAMINA_BAR;
    public TextMeshProUGUI STAMINA_UI_Txt;

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


    public Image skill_E_img;
    public Image skill_E_backimg;
    public TextMeshProUGUI skill_E_CoolTime_txt;

    public Image skill_R_img;
    public Image skill_R_backimg;

    public Image skill_F_img;
    public Image skill_F_backimg;

    public Image skill_G_img;
    public Image skill_G_backimg;

    public Sprite skill_E_sword_sp;
    public Sprite skill_E_arrow_sp;
    public Sprite skill_E_magic_sp;

    public Sprite skill_R_sp;
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
        // 각 능력치는 3번까지 업그레이드가 가능.
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
        //////////////////////////////////////////
        

        // GameManager &  Player 데이터에 기반한 UI 값들을 업데이트해줌.
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

        HP_UI.text = (int)player_data.curHP + " / " + player_data.maxHP;
        HPBAR.value = (player_data.curHP / player_data.maxHP);

        STAMINA_UI_Txt.text = (int)player_data.curStamina + " / " + player_data.maxStamina;
        STAMINA_BAR.value = (player_data.curStamina / player_data.maxStamina);

        expBar.value = (player_data.curEXP / player_data.maxEXP) * 100;
        Next_Level.text = "" + player_data.level;
        expPercent.text = string.Format("{0:0.00}", ((player_data.curEXP / player_data.maxEXP) * 100)) + "%";

        RedBuff_image.fillAmount = player_data.redBuff_time / 100f;
        BlueBuff_image.fillAmount = player_data.blueBuff_time / 100f;
        GreenBuff_image.fillAmount = player_data.greenBuff_time / 100f;


        // 직업군 마다 스킬 아이콘을 바꾸어줌.
        if (player_data.curStyle == Style.WeaponStyle.Sword)
        {
            skill_E_img.sprite = skill_E_sword_sp;
            skill_E_backimg.sprite = skill_E_sword_sp;
        }
        else if (player_data.curStyle == Style.WeaponStyle.Arrow)
        {
            skill_E_img.sprite = skill_E_arrow_sp;
            skill_E_backimg.sprite = skill_E_arrow_sp;
        }
        else if (player_data.curStyle == Style.WeaponStyle.Magic)
        {
            skill_E_img.sprite = skill_E_magic_sp;
            skill_E_backimg.sprite = skill_E_magic_sp;
        }


        // 스킬 쿨타임에 맞게 스킬 아이콘과 사용까지 남은 시간을 나타내줌.
        if (player_data.isSkill_E_Ready)
        {
            skill_E_CoolTime_txt.text = "";
            skill_E_backimg.fillAmount = 0;
        }
        else {
            skill_E_CoolTime_txt.text = "" + (int)(player_data.skill_E_cooltime - player_data.skill_E_Delay);
            skill_E_backimg.fillAmount =  (1 - (player_data.skill_E_Delay / player_data.skill_E_cooltime));
        }


        // 아레나 진입 시간에 도달하면, 아레나에 진입하는 함수를 호출
        if (time <= 0f&&player_data.isArena==false)
        {
            ArenaIn();
        }

        // 아레나 종료 시간에 도달하면, 아레나 탈출하는 함수를 호출
        if(time <= 0f && player_data.isArena==true)
        {
            ArenaOut();
        }

        // 아레나 중일때, 아레나 종료 조건이 도달했는지 체크함.
        if(player_data.isArena == true)
        {
            CheckFinished_Arena();
        }

    }
    // 아레나 입장
    void ArenaIn()
    {
        // 플레이어의 이동 입력을 초기화함.
        player_data.horizontalMove = 0f;
        player_data.verticalMove = 0f;

        player_data.arenaWin = true;

        // GameManager의 isActive를 False로 만들어
        // 플레이어를 움직이지 못하게함.
        GameManager.Instance.isActive = false;
        isActive_Input = false;

        // 아레나 병력 설정 UI를 활성화시켜줌.
        GameObject arenaCanvas = MC.transform.GetChild(7).gameObject;
        arenaCanvas.SetActive(true);
        arenaCanvas.transform.GetChild(0).gameObject.SetActive(true);
    }

    // 아레나 탈출
    public void ArenaOut()
    {
        isActive_Input = true;
        gm.arena_time = 180f;
        player_data.isArena = false;
        isArena_in = false;


        // 아레나에 남아있는 병력들을 제거해줌.
        if (player_data.PV.IsMine)
        {
            GameObject[] soldiers = GameObject.FindGameObjectsWithTag("Soldier");
            foreach (var s in soldiers)
            {
               Destroy(s);
            }
        }

        // 3초 후, 리스폰
        player_data.Invoke("Respawn",3f);

        // 아레나 결과&보상 UI를 띄어줌
        GameObject arenaCanvas = MC.transform.GetChild(7).gameObject;
        arenaCanvas.transform.GetChild(2).gameObject.SetActive(true);

        // 종료 시, 플레이어가 승리했을 경우
        if (player_data.arenaWin)
        {
            // 1등으로 처리
            player_data.arenaRank=1;

            // 마지막 아레나에서 승리했을 때, 플레이어의 최종스코어가 되는 Star 값을 두 개 올려줌.
            if(GameManager.Instance.areanaCount == 3)   //마지막 
            {
                player_data.star += 2;
            }
            else // 일반 아레나에서 승리 시, Star 값 1개 증가
            {
                player_data.star++;
            }
        }
        // 마지막 아레나일 경우, 종료했을 때 RewardManager 클래스의 Reward함수 호출을 통해서 최종 결산을 진행
        if (GameManager.Instance.areanaCount == 3)
            RewardManager.Instance.Reward();    //정산
        player_data.PV.RPC("initRank", RpcTarget.All);

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

    // 능력치 선택
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
                DefenseUp();
                def_count += 1;
                break;
            case "CRITICAL":
                CriticalUp();
                critical_count += 1;
                break;
            case "STAMINA":
                StaminaUp();
                stamina_count += 1;
                break;

        }
        LevelUpPanel.SetActive(false);

    }
    public void DefenseUp()
    {
        player_data.curDefense += 5;
    }
    public void CriticalUp()
    {
        player_data.curCritical += 15;
    }
    public void HpUp()
    {
        player_data.maxHP += 1000;
        player_data.HP_CHANGE=true;
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
        player_data.HP_CHANGE = true;
    }
    public void StaminaUp()
    {
        player_data.maxStamina += 50f;
    }

    // 선택 가능 능력치를 섞어서 3개 뽑아줌.
    IEnumerator Shuffle()
    {
        yield return new WaitForSeconds(1f);

        // 선택 가능 능력치를 bool[]에 저장함
        bool[] selected_state = { hp_possible, spd_possible, atk_possible, def_possible, critical_possible,stamina_possible };

        int count = 0;
        // 능력치 중, 3개를 뽑음
        while (count < 3)
        {
            int rand_idx = Random.Range(0, 6);
            // 선택 가능한 능력치를 대상으로 뽑음
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
