using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    public float WindPower = 1f;
    bool winding = false;
    public bool a = false;
    private void OnEnable()
    {
        a = false;
        winding = true;
        Invoke("WindOff", 5f);
    }

    void WindOff()
    {
        a = false;
        winding = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player" && winding)
        {
            a = true;
            //other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 0));
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * WindPower);
        }

    }
}
