using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Style : MonoBehaviour
{
    public enum WeaponStyle { None,Sword, Arrow, Staff };
    public WeaponStyle selectStyle;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player_Control player;
            switch (selectStyle)
            {
                case WeaponStyle.Sword:
                    player = other.gameObject.GetComponent<Player_Control>();
                    player.curStyle = WeaponStyle.Sword;
                    player.WeaponPosition_R.transform.GetChild(1).gameObject.SetActive(true);
                    player.WeaponPosition_L.transform.GetChild(1).gameObject.SetActive(true);
                    Destroy(this.gameObject, 1f);
                    break;
                case WeaponStyle.Arrow:
                    player = other.gameObject.GetComponent<Player_Control>();
                    player.curStyle = WeaponStyle.Arrow;
                    player.WeaponPosition_L.transform.GetChild(2).gameObject.SetActive(true);
                    Destroy(this.gameObject, 1f);
                    break;
                case WeaponStyle.Staff:
                    break;

            }
        }
    }
}
