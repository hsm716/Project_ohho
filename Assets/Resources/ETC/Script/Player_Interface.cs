using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Interface : MonoBehaviour
{
    public Player_Control player_data;
    public Slider expBar;
    

    // Update is called once per frame
    void Update()
    {
        expBar.value = player_data.curEXP / player_data.maxEXP;

    }
}
