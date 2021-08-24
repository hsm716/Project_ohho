using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    Animator animator;

    private void OnEnable()
    {
        StartCoroutine(BridgeCall());
    }


    IEnumerator BridgeCall()
    {

        foreach (Transform parts in transform)
        {
            animator = parts.GetChild(0).GetComponent<Animator>();
            yield return new WaitForSeconds(0.05f);
            //animator.Play("section");
            animator.SetTrigger("bridgeCall");
        }
    }
}
