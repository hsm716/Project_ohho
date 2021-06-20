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

                    break;
                case WeaponStyle.Arrow:
                    break;
                case WeaponStyle.Staff:
                    break;

            }
        }
    }
}
