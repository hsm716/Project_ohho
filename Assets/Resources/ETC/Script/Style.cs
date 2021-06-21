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
            switch (selectStyle)
            {
                case WeaponStyle.Sword:
                    Player_Control player = other.gameObject.GetComponent<Player_Control>();
                    player.curStyle = WeaponStyle.Sword;
                    player.WeaponPosition.transform.GetChild(1).gameObject.SetActive(true);
                    Destroy(this.gameObject, 1f);
                    break;
                case WeaponStyle.Arrow:
                    break;
                case WeaponStyle.Staff:
                    break;

            }
        }
    }
}
