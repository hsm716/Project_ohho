using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_HpBar : MonoBehaviourPunCallbacks
{
    public Transform monster;
    Monster monster_data;
    public Slider hpBar;
    public GameObject HpLineFolder;
    public Image hpColor;
    public PhotonView PV;

    //   private Player_Control playerLogic;
    Vector3 offset;

    bool isOnceInvoke;
    private void Awake()
    {
        
        monster_data = monster.GetComponent<Monster>();
        if (monster_data.monsterType == Monster.Type.slime)
        {
            offset = new Vector3(0f, 1f, 0f);
        }
        if (monster_data.monsterType == Monster.Type.demon)
        {
            offset = new Vector3(0f, 5f, 0f);
        }

        
        GetHpBoost();

    }

    void Update()
    {
        if (isOnceInvoke == false)
        {
            isOnceInvoke = true;
            GetHpBoost();
        }

        /*   if (Input.GetKeyDown(KeyCode.Escape))
               PV.RPC("DestroyRPC", RpcTarget.AllBuffered);*/
        transform.position = monster.position + offset;
        hpBar.value = monster_data.curHP / monster_data.maxHP;




    }
    public void GetHpBoost()
    {
        float scaleX = (1000f / 200f) / (monster_data.maxHP / 200f);
        HpLineFolder.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(false);
        foreach (Transform child in HpLineFolder.transform)
        {
            child.gameObject.transform.localScale = new Vector3(scaleX, 1, 1);
        }

        HpLineFolder.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(true);
    }
    // void DestoryRPC() => Destroy(gameObject);
}
