using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_HpBar : MonoBehaviourPunCallbacks
{
    public Transform player;
    Player_Control player_data;
    public Slider hpBar;
    public Slider subBar;
    public GameObject HpLineFolder;
    public Text Level_text;
    public PhotonView PV;

    

    //   private Player_Control playerLogic;
    Vector3 offset = new Vector3(0f, 2.3f, 0f);

    private void Awake()
    {
        player_data = player.GetComponent<Player_Control>();
        PV.RPC("GetHpBoost", RpcTarget.All);
    }

    void Update()
    {
        
        if (player_data.HP_CHANGE == true)
        {
            player_data.HP_CHANGE = false;
            PV.RPC("GetHpBoost", RpcTarget.All);
        }

        /*   if (Input.GetKeyDown(KeyCode.Escape))
               PV.RPC("DestroyRPC", RpcTarget.AllBuffered);*/
        transform.position = player.position + offset;
        Level_text.text = "" + player_data.level;
        hpBar.value = player_data.curHP / player_data.maxHP;

        if (player_data.curStyle == Style.WeaponStyle.Arrow)
        {
            subBar.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>().color = new Color(0.8f, 0.44f, 0.39f);
            subBar.value = player_data.pullPower / 40f;
        }
        else if(player_data.curStyle == Style.WeaponStyle.Sword)
        {
            subBar.value = player_data.shieldAmount / 1000f;
        }
        else if(player_data.curStyle == Style.WeaponStyle.Magic)
        {

            subBar.transform.GetChild(1).transform.GetChild(0).transform.GetComponent<Image>().color = new Color(0.38f,0.55f,0.8f);
            subBar.value = player_data.curStamina / player_data.maxStamina;
        }

        
    }
    [PunRPC]
    public void GetHpBoost()
    {
        float scaleX = (1000f / 200f) / (player_data.maxHP / 200f);
        HpLineFolder.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(false);
        foreach(Transform child in HpLineFolder.transform)
        {
            child.gameObject.transform.localScale = new Vector3(scaleX, 1, 1);
        }

        HpLineFolder.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(true);
    }
    // void DestoryRPC() => Destroy(gameObject);
}
 