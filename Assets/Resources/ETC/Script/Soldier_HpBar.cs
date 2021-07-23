using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Soldier_HpBar : MonoBehaviour
{
    public Transform soldier;
    Soldier soldier_data;
    public Slider hpBar;
    public GameObject HpLineFolder;
    //  public PhotonView PV;

    //   private Player_Control playerLogic;
    Vector3 offset = new Vector3(0f, 2.3f, 0f);

    private void Awake()
    {
        soldier_data = soldier.GetComponent<Soldier>();
        GetHpBoost();
    }

    void Update()
    {

        /*   if (Input.GetKeyDown(KeyCode.Escape))
               PV.RPC("DestroyRPC", RpcTarget.AllBuffered);*/
        transform.position = soldier.position + offset;
        hpBar.value = soldier_data.curHP / soldier_data.maxHP;




    }
    public void GetHpBoost()
    {
        float scaleX = (1000f / 200f) / (soldier_data.maxHP / 200f);
        HpLineFolder.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(false);
        foreach (Transform child in HpLineFolder.transform)
        {
            child.gameObject.transform.localScale = new Vector3(scaleX, 1, 1);
        }

        HpLineFolder.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(true);
    }
    // void DestoryRPC() => Destroy(gameObject);
}
