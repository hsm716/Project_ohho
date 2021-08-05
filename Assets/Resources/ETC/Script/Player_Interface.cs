using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Interface : MonoBehaviour
{
    public Player_Control player_data;
    public Slider expBar;


    // Update is called once per frame
    // Im so sad, wy
    void Update()
    {
        expBar.value = (player_data.curEXP / player_data.maxEXP)*100;
        Next_Level.text = ""+player_data.level;
        expPercent.text = string.Format("{0:0.00}", ((player_data.curEXP / player_data.maxEXP) * 100))  + "%";

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

    }
}
