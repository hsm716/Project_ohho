using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBall : MonoBehaviour
{
    public ParticleSystem SnowSplash;

    private void OnEnable()
    {
        StartCoroutine(Break());
    }

    IEnumerator Break()
    {
        yield return new WaitForSeconds(7.5f);
        ParticleSystem snowSP = Instantiate(SnowSplash, transform.position, Quaternion.identity);
        snowSP.transform.localScale = transform.localScale;
        //Destroy(snowSP, 3f);
        Destroy(gameObject);
    }
}
