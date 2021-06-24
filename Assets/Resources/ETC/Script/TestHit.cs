using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHit : MonoBehaviour
{
    public float curHP;
    float maxHP;
    void Start()
    {
        curHP = 200f;
        maxHP = 200f;
    }

    private void Update()
    {
        if (curHP <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player_Attack"))
        {
            curHP -= other.transform.root.GetComponent<Player_Control>().atk;
            Debug.Log("¾Æ¾æ!");
        }
    }
}
