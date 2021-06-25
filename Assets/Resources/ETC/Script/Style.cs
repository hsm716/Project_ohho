using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Style : MonoBehaviour
{
    public enum WeaponStyle { None,Sword, Arrow, Magic };
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
                    player.atk = 200f;
                    player.animator.Rebind();
                    player.animator.Play("Idle_Sword");

                    Destroy(this.gameObject, 1f);
                    break;
                case WeaponStyle.Arrow:
                    player = other.gameObject.GetComponent<Player_Control>();
                    player.curStyle = WeaponStyle.Arrow;
                    player.WeaponPosition_L.transform.GetChild(2).gameObject.SetActive(true);
                    player.animator.Rebind();
                    player.animator.Play("Idle_Arrow");
                    Destroy(this.gameObject, 1f);

                    break;
                case WeaponStyle.Magic:
                    player = other.gameObject.GetComponent<Player_Control>();
                    player.curStyle = WeaponStyle.Magic;
                    player.WeaponPosition_R.transform.GetChild(2).gameObject.SetActive(true);
                    player.animator.Rebind();
                    player.animator.Play("Idle_Magic");
                    Destroy(this.gameObject, 1f);
                    break;

            }
        }
    }
}
