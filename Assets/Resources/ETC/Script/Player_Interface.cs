using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Interface : MonoBehaviour
{
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
    string[] select_ability_names = { "HP", "SPEED", "POWER", "SHIELD", "MP" };


    public Image[] Inventory_item_img;
    public TextMeshProUGUI[] Inventory_item_txt;
    public int[] Inventory_item_num;
    public string[] Inventory_item_name;

    public Dictionary<string,int> item_Material;

    public TextMeshProUGUI time_txt;

    public float time;
    public GameObject MC;

    public GameManager gm;
    //리얼 수정좀 잘좀 해주세요;; ㅅㅂ
    // Update is called once per frame
    // Im so sad, wy

    public bool isArena = false;
    public bool isActive_Input;

    public bool isArena_in = false;

    public RawImage minimap_Renderer;

    public Image RedBuff_image;
    public Image BlueBuff_image;
    public Image GreenBuff_image;
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


    }
    void Update()
    {
        time = gm.arena_time;
        time_txt.text = string.Format("{0:00}",(int)(time/60)) + " : " + string.Format("{0:00}", (time % 60));

        if (time <= 0f&&isArena==false)
        {
            ArenaIn();
        }
        if(time <= 0f && isArena_in == true)
        {
            ArenaOut();
        }
        HP_UI.text = player_data.curHP +" / " + player_data.maxHP;
        
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

        MC.transform.GetChild(7).gameObject.SetActive(true);
    }
    void ArenaOut()
    {
        isArena_in = false;
        gm.arena_time = 300f;
        isArena = false;

        transform.position = player_data.Respawn_Center.transform.GetChild((int)(player_data.PV.ViewID / 1000)).transform.position;
    }


    public void Select(int index)
    {
        //qwe
        switch (select_name[index].text)
        {
            case "HP":
                HpUp();
                break;
            case "SPEED":
                SpeedUp();
                break;
            case "POWER":
                PowerUp();
                break;
            case "SHIELD":
                break;
            case "MP":
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

        bool[] selected_state = { false, false, false, false, false };

        int count = 0;
        while (count < 3)
        {
            int rand_idx = Random.Range(0, 5);
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
