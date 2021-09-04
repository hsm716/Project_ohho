using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // 아이템 타입 종류
    public enum Type { hp_potion,mp_potion};
    public Type type;
    public int value;
    public Sprite icon;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up);
    }
}
