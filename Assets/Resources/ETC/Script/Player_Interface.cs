using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Interface : MonoBehaviour
{
    public Player_Control player_data;
    public Slider expBar;
    public TextMeshProUGUI expPercent;

    public GameObject LevelUpPanel;
    public TextMeshProUGUI Next_Level;

    public Image[] select_img;
    //public Text[] select_name;
    public TextMeshProUGUI[] select_name;

    public Sprite[] select_ability_icons;
    string[] select_ability_names = { "HP", "SPEED", "POWER", "SHIELD", "MP" };

    // Update is called once per frame
    // Im so sad
    void Update()
    {
        expBar.value = (player_data.curEXP / player_data.maxEXP)*100;
        Next_Level.text = ""+player_data.level;
        expPercent.text = string.Format("{0:0.00}", ((player_data.curEXP / player_data.maxEXP) * 100))  + "%";

    }
    public void Select(int index)
    {
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