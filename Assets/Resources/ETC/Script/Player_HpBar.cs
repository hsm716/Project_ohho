using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_HpBar : MonoBehaviour
{
    public Transform player;
    Player_Control player_data;
    public Slider hpBar;
    public GameObject HpLineFolder;
    //  public PhotonView PV;

    //   private Player_Control playerLogic;
    Vector3 offset = new Vector3(0f, 2.3f, 0f);

    private void Awake()
    {
        player_data = player.GetComponent<Player_Control>();
    }

    void Update()
    {

        /*   if (Input.GetKeyDown(KeyCode.Escape))
               PV.RPC("DestroyRPC", RpcTarget.AllBuffered);*/
        transform.position = player.position + offset;
        hpBar.value = player_data.curHP / player_data.maxHP;
    }
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
 